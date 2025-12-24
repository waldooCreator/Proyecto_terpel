# Terpel Frontend - Integración de Ventas

Frontend moderno desarrollado en **Vue 3 + Vite + Tailwind CSS** para consumir la API de integración de ventas Terpel.

## Características

- 🔐 **Autenticación OAuth2** con mock local
- ⚡ **Procesamiento Síncrono** de archivos de ventas
- 🔄 **Procesamiento Asíncrono** con callbacks
- 🎨 **Diseño Corporativo** moderno y profesional (colores blanco y gris oscuro)
- 📱 **Responsive** y optimizado para desktop

## Requisitos Previos

- Node.js 18+ y npm
- Backend .NET corriendo en `http://localhost:5100`

## Instalación

```bash
cd frontend-vue/terpel-frontend
npm install
```

## Ejecutar en Desarrollo

```bash
npm run dev
```

La aplicación estará disponible en: **http://localhost:5173**

## Estructura del Proyecto

```
terpel-frontend/
├── src/
│   ├── components/
│   │   ├── AuthView.vue          # Componente de autenticación OAuth2
│   │   ├── SyncProcessView.vue   # Procesamiento síncrono
│   │   └── AsyncProcessView.vue  # Procesamiento asíncrono
│   ├── services/
│   │   └── apiService.js         # Servicio para llamadas HTTP al backend
│   ├── App.vue                   # Layout principal con sidebar y navegación
│   └── style.css                 # Estilos Tailwind
├── tailwind.config.js            # Configuración de Tailwind (colores corporativos)
└── package.json
```

## Uso

### 1. Autenticación

- Navega a la sección **Autenticación** en el sidebar
- Usa las credenciales de prueba precargadas:
  - Client ID: `terpel_test_client`
  - Client Secret: `s3cr3t_Terpel!2025`
- Haz clic en **Obtener Token**
- El sistema te redirigirá automáticamente al procesamiento síncrono

### 2. Procesamiento Síncrono

- Ingresa la URL del archivo (por defecto: `dummy://local/dummy.csv`)
- Selecciona el tipo de autenticación (OAuth 2.0)
- Opcionalmente configura una URL de callback
- Haz clic en **🚀 Procesar Archivo**
- Los resultados se mostrarán inmediatamente con:
  - ID de transacción
  - Cantidad de registros válidos e inválidos
  - Detalle de cada registro procesado

### 3. Procesamiento Asíncrono

- Similar al síncrono, pero el procesamiento ocurre en segundo plano
- Recibirás un ID de transacción inmediatamente
- El resultado se notificará al callback configurado
- El historial de procesamientos se muestra en la misma pantalla

## Colores Corporativos

El diseño usa una paleta corporativa definida en `tailwind.config.js`:

- `corporate-dark`: #1a1a1a (sidebar, botones principales)
- `corporate-medium`: #2d2d2d (hover states)
- `corporate-light`: #404040
- `corporate-accent`: #666666

## Build para Producción

```bash
npm run build
```

Los archivos optimizados estarán en `dist/`

## Solución de Problemas

### El backend no responde

Verifica que la API .NET esté corriendo:

```bash
ss -ltnp | grep ':5100\b'
```

Si no está corriendo, inicia el backend desde la raíz del proyecto:

```bash
cd ~/TerpelIntegracion
nohup dotnet run --project Presentation --no-build --urls "http://localhost:5100" > /tmp/presentation.log 2>&1 &
```

### Error de CORS

El backend debe tener CORS habilitado para `http://localhost:5173`. Esto ya está configurado en `Presentation/Program.cs`.

### No se puede autenticar

Asegúrate de que:
1. El backend esté corriendo
2. El endpoint `/oauth/token` esté disponible
3. Las credenciales sean correctas

## Tecnologías

- **Vue 3** - Framework progresivo de JavaScript
- **Vite** - Build tool ultra-rápido
- **Tailwind CSS** - Framework de utilidades CSS
- **Axios** - Cliente HTTP para llamadas a la API

## Próximas Mejoras

- [ ] Persistencia del token en localStorage
- [ ] Polling para verificar estado de procesamientos asíncronos
- [ ] Carga de archivos desde el navegador (upload)
- [ ] Dashboard con métricas y gráficos
- [ ] Modo oscuro
