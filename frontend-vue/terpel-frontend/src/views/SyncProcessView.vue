<template>
  <section class="bg-white border border-gray-200 rounded-2xl shadow-xl p-6 md:p-8 space-y-5">
    <header class="space-y-2">
      <div class="flex items-center gap-3">
        <span class="h-10 w-10 rounded-xl bg-red-50 border border-red-100 text-red-600 font-bold inline-flex items-center justify-center">1</span>
        <div>
          <p class="text-xs uppercase tracking-[0.28em] text-red-600">Proceso síncrono</p>
          <h2 class="text-2xl font-bold text-gray-900">Consumir /api/terpel/ventas/sync</h2>
        </div>
      </div>
      <p class="text-sm text-gray-600">Procesa y devuelve registros válidos e inválidos en la misma solicitud.</p>
    </header>

    <form class="grid gap-4 md:grid-cols-3 bg-gray-50 border border-gray-200 rounded-xl p-4 md:p-5" @submit.prevent="handleProcess">
      <label class="space-y-2 text-sm font-semibold text-gray-800">
        DynamicUrl
        <input
          v-model="dynamicUrl"
          type="text"
          class="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:ring-2 focus:ring-red-500 focus:border-red-500 transition"
          placeholder="real o dummy://local/dummy.csv"
        />
      </label>

      <label class="space-y-2 text-sm font-semibold text-gray-800">
        AuthType
        <select
          v-model="authType"
          class="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:ring-2 focus:ring-red-500 focus:border-red-500 transition"
        >
          <option value="bearer">bearer</option>
          <option value="OAuth">OAuth</option>
          <option value="ApiKey">ApiKey</option>
        </select>
      </label>

      <label class="space-y-2 text-sm font-semibold text-gray-800">
        CallbackUrl (opcional)
        <input
          v-model="callbackUrl"
          type="url"
          class="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:ring-2 focus:ring-red-500 focus:border-red-500 transition"
          placeholder="http://localhost:5113/api/terpel/callback/success"
        />
      </label>

      <label class="space-y-2 text-sm font-semibold text-gray-800">
        EDS Id (obligatorio para enviar a PHP)
        <input
          v-model.number="edsId"
          type="number"
          min="1"
          class="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:ring-2 focus:ring-red-500 focus:border-red-500 transition"
          placeholder="416"
          required
        />
      </label>

      <label class="space-y-2 text-sm font-semibold text-gray-800">
        Fecha del archivo
        <input
          v-model="fechaArchivo"
          type="date"
          class="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:ring-2 focus:ring-red-500 focus:border-red-500 transition"
        />
      </label>

      <label class="space-y-2 text-sm font-semibold text-gray-800">
        Nombre de archivo
        <input
          v-model="nombreArchivo"
          type="text"
          class="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:ring-2 focus:ring-red-500 focus:border-red-500 transition"
          placeholder="ventas.csv"
        />
      </label>

      <div class="md:col-span-3 flex flex-wrap items-center gap-4 pt-2">
        <button
          type="submit"
          class="bg-gradient-to-r from-red-600 to-red-500 hover:from-red-700 hover:to-red-600 focus:ring-4 focus:ring-yellow-300 text-white font-semibold px-6 py-3 rounded-xl shadow-lg transition transform hover:-translate-y-0.5 disabled:opacity-70 disabled:cursor-not-allowed"
          :disabled="loading"
        >
          <span v-if="loading">Procesando...</span>
          <span v-else>Ejecutar sync</span>
        </button>
        <p class="text-sm text-gray-600">Envía DynamicUrl, AuthType y CallbackUrl al backend local.</p>
      </div>
    </form>

    <p v-if="errorMessage" class="bg-red-50 border border-red-200 text-red-700 text-sm rounded-lg px-4 py-3 shadow-sm">
      {{ errorMessage }}
    </p>

    <p v-if="phpMessage" class="bg-blue-50 border border-blue-200 text-blue-800 text-sm rounded-lg px-4 py-3 shadow-sm">
      {{ phpMessage }}
    </p>

    <section v-if="result" class="space-y-4">
      <div class="grid gap-4 md:grid-cols-3">
        <div class="bg-gradient-to-br from-gray-50 to-white border border-gray-200 rounded-xl p-4 shadow-sm">
          <p class="text-xs uppercase tracking-[0.2em] text-gray-500">EDS</p>
          <p class="text-lg font-bold text-gray-900">{{ result.idTransaccion || 'N/D' }}</p>
        </div>
        <div class="bg-gradient-to-br from-green-50 to-white border border-green-200 rounded-xl p-4 shadow-sm">
          <p class="text-xs uppercase tracking-[0.2em] text-green-700">Válidos</p>
          <p class="text-2xl font-bold text-green-700">{{ validos.length }}</p>
        </div>
        <div class="bg-gradient-to-br from-amber-50 to-white border border-amber-200 rounded-xl p-4 shadow-sm">
          <p class="text-xs uppercase tracking-[0.2em] text-amber-700">Inválidos</p>
          <p class="text-2xl font-bold text-amber-700">{{ invalidos.length }}</p>
        </div>
      </div>

      <article v-if="validos.length" class="bg-white border border-gray-200 rounded-xl shadow-lg p-4 space-y-3">
        <div class="flex flex-wrap items-center justify-between gap-3">
          <div>
            <h3 class="text-sm font-bold text-gray-800">Tabla de registros válidos</h3>
            <p class="text-xs text-gray-500">Campos clave y sumatoria de ingresos.</p>
          </div>
          <div class="px-3 py-1 rounded-full bg-green-50 border border-green-200 text-sm font-semibold text-green-700">
            Total ingresos: {{ formatCurrency(totalIngresos) }}
          </div>
        </div>

        <div class="overflow-auto rounded-lg border border-gray-100 shadow-inner">
          <table class="min-w-full text-sm text-gray-800">
            <thead class="bg-gradient-to-r from-gray-50 to-white text-gray-600">
              <tr>
                <th v-for="header in tableHeaders" :key="header" class="px-3 py-2 text-left font-semibold whitespace-nowrap">{{ header }}</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, idx) in validos" :key="item.id || idx" class="border-b border-gray-50 hover:bg-gray-50">
                <td v-for="header in tableHeaders" :key="header" class="px-3 py-2 whitespace-nowrap">
                  {{ item[header] ?? '' }}
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </article>

      <div class="flex flex-wrap gap-3">
        <button
          class="border border-gray-300 text-gray-800 rounded-lg px-4 py-2 text-sm font-semibold shadow-sm disabled:opacity-60 disabled:cursor-not-allowed"
          :disabled="!validos.length"
          @click="exportValidos"
        >
          Exportar válidos (JSON)
        </button>
        <span class="text-sm text-gray-500">Respuesta cruda disponible en consola.</span>
      </div>

      <div class="grid gap-4 md:grid-cols-2">
        <article class="bg-white border border-gray-200 rounded-xl shadow-sm p-4 space-y-2">
          <h3 class="text-sm font-bold text-gray-800">Registros válidos</h3>
          <p v-if="!validos.length" class="text-sm text-gray-500">Sin registros válidos.</p>
          <ul v-else class="space-y-2 text-sm text-gray-700 max-h-80 overflow-auto">
            <li v-for="(item, idx) in validos" :key="idx" class="border-b border-gray-100 pb-2">
              <pre class="whitespace-pre-wrap text-xs">{{ item }}</pre>
            </li>
          </ul>
        </article>

        <article class="bg-white border border-gray-200 rounded-xl shadow-sm p-4 space-y-2">
          <h3 class="text-sm font-bold text-gray-800">Registros inválidos</h3>
          <p v-if="!invalidos.length" class="text-sm text-gray-500">Sin registros inválidos.</p>
          <ul v-else class="space-y-2 text-sm text-gray-700 max-h-80 overflow-auto">
            <li v-for="(item, idx) in invalidos" :key="idx" class="border-b border-gray-100 pb-2">
              <pre class="whitespace-pre-wrap text-xs">{{ item }}</pre>
            </li>
          </ul>
        </article>
      </div>
    </section>
  </section>
</template>

<script setup>
import { computed, ref } from 'vue'
import Papa from 'papaparse'
import axios from 'axios'
import { processSyncFile, getConsolidadosEDS, syncToPhp } from '../services/apiService'

const dynamicUrl = ref('real')
const authType = ref('bearer')
const callbackUrl = ref('http://localhost:5113/api/terpel/callback/success')
const edsId = ref(0)
const fechaArchivo = ref(new Date().toISOString().slice(0, 10))
const nombreArchivo = ref('ventas.csv')

const loading = ref(false)
const errorMessage = ref('')
const result = ref(null)
const archivos = ref([])
const filas = ref([])
const phpMessage = ref('')
const phpSending = ref(false)

const validos = computed(() => filas.value)
const invalidos = computed(() => [])
const tableHeaders = computed(() => {
  const seen = new Set()
  const ordered = []
  filas.value.forEach((row) => {
    Object.keys(row).forEach((key) => {
      if (!seen.has(key)) {
        seen.add(key)
        ordered.push(key)
      }
    })
  })
  return ordered
})
const totalIngresos = computed(() => validos.value.reduce((sum, v) => sum + precioUnitario(v), 0))

const handleProcess = async () => {
  loading.value = true
  errorMessage.value = ''
  phpMessage.value = ''
  result.value = null
  archivos.value = []
  filas.value = []
  try {
    const syncResponse = await processSyncFile(dynamicUrl.value, authType.value, callbackUrl.value || undefined)

    // Usar inmediatamente la respuesta del backend (ya parseada)
    const parsedValidos = syncResponse?.registrosValidos || []
    filas.value = parsedValidos
    result.value = {
      idTransaccion: syncResponse?.idTransaccion || syncResponse?.eds_id || 'N/D',
      registrosValidos: parsedValidos,
      registrosInvalidos: syncResponse?.registrosInvalidos || []
    }

    // Intentar (opcional) descargar los CSV firmados si el endpoint existe; ignorar 404.
    try {
      const meta = await getConsolidadosEDS()
      archivos.value = meta?.archivos || meta?.data?.archivos || []

      if (archivos.value.length) {
        const descargas = archivos.value.map(async (file) => {
          const resp = await axios.get(file.signed_url, { responseType: 'text' })
          const parsed = Papa.parse(resp.data, { header: true, skipEmptyLines: true })
          return parsed.data
        })

        const resultados = await Promise.all(descargas)
        const flat = resultados.flat().filter(Boolean)
        filas.value = flat
        result.value = {
          idTransaccion: meta?.eds_id || meta?.nombre_eds || syncResponse?.idTransaccion || 'N/D',
          registrosValidos: flat,
          registrosInvalidos: []
        }
        console.info('CSV filas combinadas', flat.length)
      }
    } catch (optionalErr) {
      console.warn('Consolidado opcional no disponible:', optionalErr.message)
    }

    // Enviar al PHP con el dataset final (filas.value)
    await sendToPhp(filas.value)
  } catch (error) {
    errorMessage.value = error.message || 'Error al procesar la solicitud'
  } finally {
    loading.value = false
  }
}

const sendToPhp = async (rows) => {
  if (!rows || !rows.length) {
    phpMessage.value = 'No hay registros para enviar al PHP'
    return
  }
  if (!edsId.value || edsId.value <= 0) {
    phpMessage.value = 'Debes ingresar un EDS Id válido antes de enviar al PHP'
    return
  }
  phpSending.value = true
  try {
    const payload = {
      edsId: edsId.value,
      job: {
        edsId: edsId.value,
        totalArchivos: 1,
        urlsGeneradas: 1,
        expiracionMinutos: 60,
        status: 1,
        errorMessage: ''
      },
      archivo: {
        nombre: nombreArchivo.value || 'ventas.csv',
        tipo: 'ventas',
        fecha: fechaArchivo.value,
        url: dynamicUrl.value,
        status: 'procesando'
      },
      ventas: rows
    }

    const resp = await syncToPhp(payload)
    const enviados = resp?.enviados ?? 0
    phpMessage.value = resp?.mensaje || `Enviados al PHP: ${enviados}`
  } catch (err) {
    phpMessage.value = `Error enviando al PHP: ${err.message}`
  } finally {
    phpSending.value = false
  }
}

const exportValidos = () => {
  if (!validos.value.length) return
  const blob = new Blob([JSON.stringify(validos.value, null, 2)], { type: 'application/json' })
  const url = URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = 'registros-validos.json'
  a.click()
  URL.revokeObjectURL(url)
}

const normalizeNumber = (value) => {
  if (typeof value === 'number') return value
  if (typeof value === 'string') {
    const cleaned = value.replace(/[^0-9.-]/g, '')
    const parsed = Number(cleaned)
    return Number.isNaN(parsed) ? 0 : parsed
  }
  return 0
}

const precioUnitario = (item) => {
  const raw =
    item?.['Precio unitario de venta'] ??
    item?.['Precio'] ??
    item?.['Valor venta'] ??
    item?.precioUnitarioVenta ??
    item?.monto ??
    item?.Monto ??
    0
  return normalizeNumber(raw)
}

const formatCurrency = (value) =>
  new Intl.NumberFormat('es-CO', { style: 'currency', currency: 'COP', minimumFractionDigits: 0 }).format(value || 0)
</script>
