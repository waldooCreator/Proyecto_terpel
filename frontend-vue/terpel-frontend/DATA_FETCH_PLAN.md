# Plan de consumo de datos (frontend desde cero)

## Estado actual (cómo consume hoy)
- Base API: `http://localhost:5113`
- Servicio central: `src/services/apiService.js`
  - `processSyncFile(dynamicUrl, authType='bearer', callbackUrl)`: POST `/api/terpel/ventas/sync` con body `{ DynamicUrl, AuthType, CallbackUrl }`. Devuelve registros válidos/ inválidos y `idTransaccion`.
  - `processAsyncFile(dynamicUrl, authType='OAuth', callbackUrl)`: POST `/api/terpel/ventas/async` con el mismo body. Devuelve solo `idTransaccion` (proceso en background).
- Flujo síncrono (`SyncProcessView.vue`):
  - Controles: `dynamicUrl` (real vs dummy), `authType` (bearer/OAuth/ApiKey), `callbackUrl` (default `http://localhost:5113/api/terpel/callback/success`).
  - Acción: `handleProcess()` llama `processSyncFile()` y muestra totales, válidos, inválidos; permite exportar válidos a JSON.
- Flujo asíncrono (`AsyncProcessView.vue`):
  - Controles: `dynamicUrl`, `authType`, `callbackUrl` requerido.
  - Acción: `handleProcess()` llama `processAsyncFile()`, muestra `idTransaccion`, agrega a historial local y espera callback externo.
- Enrutado: `src/router/index.js` expone una sola ruta `/` que renderiza `App.vue`, y este intercambia entre `SyncProcessView` y `AsyncProcessView` vía `currentView` local.

## Datos mínimos para consumir la API
- URL base backend: `http://localhost:5113`
- Endpoints:
  - Sync: `POST /api/terpel/ventas/sync`
  - Async: `POST /api/terpel/ventas/async`
- Payload esperado (ambos):
  ```json
  {
    "DynamicUrl": "real" | "dummy://local/dummy.csv" | "<url>",
    "AuthType": "bearer" | "OAuth" | "ApiKey",
    "CallbackUrl": "http://.../callback" // requerido para async, opcional en sync
  }
  ```
- Respuesta sync: `{ idTransaccion, registrosValidos: [], registrosInvalidos: [] }`
- Respuesta async: `{ idTransaccion }`

## Pasos para rehacer el frontend (consumo de datos)
1) Configurar axios
- Crear cliente con `baseURL` al backend y header `Content-Type: application/json`.
- Opcional: interceptores para logs y manejo de errores.

2) Implementar servicio de API
- Métodos: `processSyncFile(body)` y `processAsyncFile(body)` que llamen a los endpoints anteriores.
- Aceptar parámetros `dynamicUrl`, `authType`, `callbackUrl` y armar el payload.

3) Crear estados de UI
- `loading`, `errorMessage`, `result` (sync) o `acceptedResponse/history` (async).
- Controles de formulario para `dynamicUrl`, `authType`, `callbackUrl`.

4) Disparar solicitudes
- En submit, set `loading=true`, limpiar errores y resultados.
- Llamar servicio y manejar respuesta: mostrar totales (sync) o ID de transacción (async).
- Capturar errores: `error.response?.data?.message || error.message`.

5) Mostrar datos
- Sync: tarjetas de totales, listas de válidos/ inválidos, botón export JSON de válidos.
- Async: mostrar `idTransaccion` y callback destino; opcional polling si se desea estatus.

6) Opcionales de resiliencia
- `dynamicUrl` preset: `real` para API real, `dummy://local/dummy.csv` para pruebas.
- Validar `callbackUrl` requerido en async.
- Manejar expiración de token (si se agrega flujo OAuth real).

## Ejemplo de uso del servicio
```javascript
import api from './services/apiService'

// Síncrono
const sync = await api.processSyncFile('real', 'bearer', 'http://localhost:5113/api/terpel/callback/success')
console.log(sync.registrosValidos.length, sync.registrosInvalidos.length)

// Asíncrono
const asyncResp = await api.processAsyncFile('real', 'bearer', 'http://localhost:5113/api/terpel/callback/success')
console.log(asyncResp.idTransaccion)
```

## Checklist para un frontend nuevo
- [ ] Definir `.env` con `VITE_API_BASE_URL`
- [ ] Crear cliente axios con baseURL y headers
- [ ] Implementar servicio con los dos métodos (sync/async)
- [ ] Formularios para `dynamicUrl`, `authType`, `callbackUrl`
- [ ] Estado `loading` y `errorMessage`
- [ ] Render de resultados (sync) y de aceptación (async)
- [ ] Exportación de válidos a JSON (sync)
- [ ] Validar campos requeridos y deshabilitar botón mientras carga

## Notas rápidas
- El backend ya maneja `dynamicUrl="real"` para llamar al API Terpel y obtener la signed URL.
- No agregar Authorization al signed URL: se descarga directo.
- Token Bearer y URL de Terpel se configuran en backend (`appsettings.json`).
