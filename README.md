# Terpel Integración — Resumen y Guía de Pruebas

Última actualización: 30 de noviembre de 2025

## Propósito

Servicio .NET para integrarse con la plataforma de Terpel y consumir archivos diarios de ventas (XLS/CSV). El sistema descarga el archivo, lo valida, lo mapea a entidades internas y expone operaciones REST para ejecución síncrona y asíncrona (con callback). Implementación con Clean Architecture (Domain / Application / Infrastructure / Presentation).

## Qué hace el sistema (alto nivel)

- Consulta una URL dinámica publicada por Terpel (placeholder).
- Descarga el archivo (XLS/CSV) desde la URL final.
- Parsea el archivo a una colección de `RegistroVenta` (modelo con 28 campos exactos definidos por Terpel).
- Valida cada registro con `FluentValidation`.
- Mapea DTOs a entidades de dominio con `AutoMapper`.
- Registra auditoría y logs estructurados (`idTransaccion`, operaciones, request/response parciales).
- Soporta ejecución:
  - Síncrona: retorna resultado inmediato con registros válidos e inválidos.
  - Asíncrona: inicia procesamiento en background y notifica por `CallbackUrl` (success/error) al completar.

## Estructura principal del repo

- `Domain/` — Entidades de dominio (`RegistroVentaEDS`).
- `Application/` — DTOs, interfaces, servicios de aplicación, validadores y mapeos.
- `Infrastructure/` — Implementaciones (HTTP client placeholder, parser CSV, posibilidad de ExcelDataReader).
- `Presentation/` — API REST (controladores), middleware de errores y `Program.cs` con DI y Swagger.

## Endpoints expuestos

- `POST /api/terpel/ventas/sync` — Ejecuta procesamiento y retorna `ProcessResultDto`.
- `POST /api/terpel/ventas/async` — Inicia procesamiento asíncrono; retorna `202 Accepted` con `idTransaccion`.
- `POST /api/terpel/callback/success` — Endpoint simulado para recibir callback de éxito.
- `POST /api/terpel/callback/error` — Endpoint simulado para recibir callback de error.

## Flujo de procesamiento (paso a paso)

1. Servicio llama `ITerpelClient.GetDynamicUrlAsync(dynamicPlaceholder)` para resolver URL final.
2. Descarga mediante `ITerpelClient.DownloadFileAsync(fileUrl)` → `Stream`.
3. Parseo con `IFileParser.ParseAsync(stream, fileName)` → `List<RegistroVentaDto>`.
4. Validación registro a registro con `RegistroVentaValidator`.
5. Resultado: `ProcessResultDto` con `idTransaccion`, `registrosValidos` y `registrosInvalidos`.
6. En modo asíncrono, se ejecuta el mismo flujo en background y, si existe `CallbackUrl`, se hace un `POST` con el resultado (o con el error) al callback.

## Cómo ejecutar y probar (desde terminal)

> Antes: asegúrate de tener el .NET SDK compatible instalado. El proyecto apunta a `net10.0`.

1. Compilar la solución:

```bash
cd /home/wall-do/TerpelIntegracion
dotnet build
```

2. Ejecutar la API (foreground):

```bash
dotnet run --project Presentation --urls "http://localhost:5000"
```

Opcional: ejecutar en background y volcar logs:

```bash
nohup dotnet run --project Presentation --no-build --urls "http://localhost:5000" > /tmp/presentation.log 2>&1 &
# anota el PID devuelto
sleep 1
tail -n 50 /tmp/presentation.log
```

3. Verificar que el servicio está escuchando en el puerto 5000:

```bash
ss -ltnp | grep ':5000\b' || true
```

4. Verificar Swagger JSON y que las rutas existen:

```bash
curl -sS http://localhost:5000/swagger/v1/swagger.json -o /tmp/swagger.json
grep -nE '"/api/terpel/ventas/sync"|"/api/terpel/ventas/async"|"/api/terpel/callback/success"|"/api/terpel/callback/error"' /tmp/swagger.json || true
```

5. Probar endpoint síncrono (payload `dummy`):

```bash
curl -sS -X POST http://localhost:5000/api/terpel/ventas/sync \
  -H "Content-Type: application/json" \
  -d '{"DynamicUrl":"dummy://local/dummy.csv","AuthType":"ApiKey","CallbackUrl":"http://localhost:5000/api/terpel/callback/success"}' \
  -w "\nHTTP_STATUS:%{http_code}\n" -o /tmp/sync_response.json

jq . /tmp/sync_response.json || cat /tmp/sync_response.json
tail -n 80 /tmp/presentation.log
```

6. Probar endpoint asíncrono (y confirmar callback):

```bash
curl -sS -X POST http://localhost:5000/api/terpel/ventas/async \
  -H "Content-Type: application/json" \
  -d '{"DynamicUrl":"dummy://local/dummy.csv","AuthType":"ApiKey","CallbackUrl":"http://localhost:5000/api/terpel/callback/success"}' \
  -w "\nHTTP_STATUS:%{http_code}\n" -o /tmp/async_response.json

jq . /tmp/async_response.json || cat /tmp/async_response.json
sleep 2
tail -n 120 /tmp/presentation.log
```

7. Comprobar errores o excepciones en logs:

```bash
grep -nEi 'exception|error|unable to resolve|missingmethod' /tmp/presentation.log || true
```

8. Parar la API (si está en background):

```bash
ss -ltnp | grep ':5000\b' || true
# luego kill <PID> o
pkill -f "dotnet run --project Presentation" || true
```

### Si el puerto `5000` ya está en uso

Si al arrancar la aplicación obtienes un error "Address already in use" o el puerto 5000 está ocupado, sigue estos pasos en orden.

1. Identificar el proceso que usa el puerto 5000:

```bash
ss -ltnp | grep ':5000\b' || true
# o (requiere sudo):
sudo lsof -iTCP:5000 -sTCP:LISTEN -P -n || true
```

2. Inspeccionar el proceso antes de terminarlo (reemplaza `<PID>`):

```bash
ps -p <PID> -o pid,cmd,etime
```

3. Liberar el puerto (si es seguro terminar el proceso):

```bash
# señal suave
kill <PID>
sleep 1
ss -ltnp | grep ':5000\b' || true

# forzar si no responde
kill -9 <PID>
ss -ltnp | grep ':5000\b' || true
```

4. Alternativa: ejecutar la app en otro puerto (recomendado si no quieres matar procesos):

```bash
dotnet run --project Presentation --urls "http://localhost:5100"
# o en background
nohup dotnet run --project Presentation --no-build --urls "http://localhost:5100" > /tmp/presentation.log 2>&1 &
```

5. Después de liberar el puerto o cambiar a otro, verifica arranque y logs:

```bash
sleep 1
tail -n 80 /tmp/presentation.log
ss -ltnp | grep ':5000\b' || ss -ltnp | grep ':5100\b' || true
```

Nota: solo termina procesos que identifiques como tus pruebas (por ejemplo, procesos `dotnet` o `Presentation`). No mates procesos del sistema que puedas necesitar.


## Archivos útiles generados en pruebas

- Logs del servidor (si ejecutas con nohup): `/tmp/presentation.log`
- Swagger JSON: `/tmp/swagger.json`
- Respuestas de prueba: `/tmp/sync_response.json`, `/tmp/async_response.json`

## Limitaciones y notas

- Autenticación (OAuth / API Key / Basic) no implementada: la integración real debe implementarse en `Infrastructure/Services/TerpelHttpService.cs`.
- Parser actual soporta CSV con encabezado que coincida exactamente con los 28 nombres de columna. Para XLS/XLSX hay paquetes (`ExcelDataReader`) ya referenciados y listo para integrarse.
- Algunas propiedades del DTO son no-nullable; en las pruebas incluimos `CallbackUrl` para evitar `400 Bad Request` por binding.
- AutoMapper fue alineado en versión para evitar `MissingMethodException` en tiempo de ejecución.

## Próximos pasos sugeridos

- Activar soporte XLS/XLSX en `FileParser` usando `ExcelDataReader`.
- Añadir la lógica de autenticación (OAuth client credentials, API key o Basic) en `TerpelHttpService` con configuración segura (secrets/env).
- Mejorar logs con `Serilog` y sink a archivo/ELK, y añadir campos de auditoría obligatorios (idTransaccion, nombreServicio, nombreOperacion, fechaEjecucion, request, response, aplicacion, dispositivo).
- Añadir pruebas unitarias y de integración más completas para el flujo de parsing y validación.

---
Archivo generado: `README.md` (resumen, guía de pruebas y comandos). Si quieres que lo amplíe (por ejemplo con ejemplos de payload, o scripts `curl` listos), dime exactamente qué agregar.


## Urls Prueba

- http://localhost:5000/swagger/v1/swagger.json

- http://localhost:5000/swagger/index.html