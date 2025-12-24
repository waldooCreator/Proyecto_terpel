<template>
  <section class="bg-white border border-gray-200 rounded-2xl shadow-xl p-6 md:p-8 space-y-5">
    <header class="space-y-2">
      <div class="flex items-center gap-3">
        <span class="h-10 w-10 rounded-xl bg-red-50 border border-red-100 text-red-600 font-bold inline-flex items-center justify-center">2</span>
        <div>
          <p class="text-xs uppercase tracking-[0.28em] text-red-600">Proceso asíncrono</p>
          <h2 class="text-2xl font-bold text-gray-900">Consumir /api/terpel/ventas/async</h2>
        </div>
      </div>
      <p class="text-sm text-gray-600">Envía la solicitud y espera el callback externo; el backend responde con un idTransaccion.</p>
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
          <option value="OAuth">OAuth</option>
          <option value="bearer">bearer</option>
          <option value="ApiKey">ApiKey</option>
        </select>
      </label>

      <label class="space-y-2 text-sm font-semibold text-gray-800">
        CallbackUrl (requerido)
        <input
          v-model="callbackUrl"
          type="url"
          required
          class="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:ring-2 focus:ring-red-500 focus:border-red-500 transition"
          placeholder="http://localhost:5113/api/terpel/callback/success"
        />
      </label>

      <div class="md:col-span-3 flex flex-wrap items-center gap-4 pt-2">
        <button
          type="submit"
          class="bg-gradient-to-r from-red-600 to-red-500 hover:from-red-700 hover:to-red-600 focus:ring-4 focus:ring-yellow-300 text-white font-semibold px-6 py-3 rounded-xl shadow-lg transition transform hover:-translate-y-0.5 disabled:opacity-70 disabled:cursor-not-allowed"
          :disabled="loading"
        >
          <span v-if="loading">Enviando...</span>
          <span v-else>Ejecutar async</span>
        </button>
        <p class="text-sm text-gray-600">Recibirás solo idTransaccion; el procesamiento continúa en background.</p>
      </div>
    </form>

    <p v-if="errorMessage" class="bg-red-50 border border-red-200 text-red-700 text-sm rounded-lg px-4 py-3 shadow-sm">
      {{ errorMessage }}
    </p>

    <section v-if="acceptedResponse" class="bg-gradient-to-r from-yellow-50 to-white border border-yellow-200 rounded-xl p-4 space-y-2 shadow-sm">
      <p class="text-xs uppercase tracking-[0.2em] text-red-700">Solicitud aceptada</p>
      <p class="text-lg font-bold text-red-800">idTransaccion: {{ acceptedResponse.idTransaccion }}</p>
      <p class="text-sm text-red-700">Callback enviado a: {{ callbackUrl }}</p>
    </section>

    <section v-if="history.length" class="space-y-3">
      <div class="flex items-center gap-2">
        <p class="text-sm font-semibold text-gray-800">Historial local</p>
        <span class="text-xs text-gray-500">Últimas solicitudes async enviadas desde el navegador.</span>
      </div>
      <ul class="space-y-2 text-sm text-gray-700">
        <li v-for="item in history" :key="item.timestamp" class="border border-gray-200 rounded-lg px-4 py-3 bg-white shadow-sm">
          <div class="flex flex-wrap gap-2 justify-between">
            <span class="font-semibold text-gray-900">{{ item.idTransaccion }}</span>
            <span class="text-xs text-gray-500">{{ item.timestamp }}</span>
          </div>
          <p class="text-xs text-gray-600">DynamicUrl: {{ item.dynamicUrl }} · Auth: {{ item.authType }} · Callback: {{ item.callbackUrl }}</p>
        </li>
      </ul>
    </section>
  </section>
</template>

<script setup>
import { ref } from 'vue'
import { processAsyncFile } from '../services/apiService'

const dynamicUrl = ref('real')
const authType = ref('OAuth')
const callbackUrl = ref('http://localhost:5113/api/terpel/callback/success')

const loading = ref(false)
const errorMessage = ref('')
const acceptedResponse = ref(null)
const history = ref([])

const handleProcess = async () => {
  if (!callbackUrl.value) {
    errorMessage.value = 'CallbackUrl es requerido para el proceso asíncrono'
    return
  }

  loading.value = true
  errorMessage.value = ''
  acceptedResponse.value = null
  try {
    const response = await processAsyncFile(dynamicUrl.value, authType.value, callbackUrl.value)
    acceptedResponse.value = response
    history.value.unshift({
      idTransaccion: response.idTransaccion || 'N/D',
      dynamicUrl: dynamicUrl.value,
      authType: authType.value,
      callbackUrl: callbackUrl.value,
      timestamp: new Date().toLocaleString()
    })
    console.info('Async response', response)
  } catch (error) {
    errorMessage.value = error.message || 'Error al enviar la solicitud asíncrona'
  } finally {
    loading.value = false
  }
}
</script>
