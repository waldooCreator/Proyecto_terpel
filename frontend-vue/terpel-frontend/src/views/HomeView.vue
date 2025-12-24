<template>
  <div class="min-h-screen bg-gradient-to-br from-gray-50 via-gray-100 to-red-50">
    <!-- Decorative Background Elements -->
    <div class="fixed inset-0 overflow-hidden pointer-events-none">
      <div class="absolute -top-24 -right-24 w-96 h-96 bg-red-500/5 rounded-full blur-3xl"></div>
      <div class="absolute top-1/2 -left-32 w-80 h-80 bg-yellow-500/5 rounded-full blur-3xl"></div>
      <div class="absolute bottom-0 right-1/4 w-72 h-72 bg-red-600/5 rounded-full blur-3xl"></div>
    </div>

    <!-- Header -->
    <header class="relative bg-gradient-to-br from-red-700 via-red-600 to-red-800 text-white overflow-hidden mb-12">
      <div class="absolute inset-0 bg-[url('data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNjAiIGhlaWdodD0iNjAiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyI+PGRlZnM+PHBhdHRlcm4gaWQ9ImdyaWQiIHdpZHRoPSI2MCIgaGVpZ2h0PSI2MCIgcGF0dGVyblVuaXRzPSJ1c2VyU3BhY2VPblVzZSI+PHBhdGggZD0iTSAxMCAwIEwgMCAwIDAgMTAiIGZpbGw9Im5vbmUiIHN0cm9rZT0id2hpdGUiIHN0cm9rZS1vcGFjaXR5PSIwLjAzIiBzdHJva2Utd2lkdGg9IjEiLz48L3BhdHRlcm4+PC9kZWZzPjxyZWN0IHdpZHRoPSIxMDAlIiBoZWlnaHQ9IjEwMCUiIGZpbGw9InVybCgjZ3JpZCkiLz48L3N2Zz4=')] opacity-50"></div>
      
      <div class="relative max-w-7xl mx-auto px-6 py-12">
        <div class="flex items-start justify-between">
          <div class="space-y-4">
            <div class="inline-flex items-center gap-2 bg-yellow-400/20 backdrop-blur-sm border border-yellow-400/30 rounded-full px-4 py-1.5">
              <div class="w-2 h-2 bg-yellow-400 rounded-full animate-pulse"></div>
              <span class="text-xs font-bold tracking-[0.3em] text-yellow-200 uppercase">
                Plan de Consumo de Datos
              </span>
            </div>
            
            <h1 class="text-5xl md:text-6xl font-black tracking-tight">
              <span class="bg-gradient-to-r from-white to-yellow-100 bg-clip-text text-transparent">
                Frontend
              </span>
            </h1>
            
            <p class="text-yellow-100/80 text-sm font-medium">
              Payload: <span class="text-white font-semibold">DynamicUrl</span> / 
              <span class="text-white font-semibold"> AuthType</span> / 
              <span class="text-white font-semibold"> CallbackUrl</span>
            </p>
          </div>
          
          <div class="hidden md:block bg-white/10 backdrop-blur-md border border-white/20 rounded-2xl px-5 py-3 shadow-2xl">
            <div class="text-xs text-yellow-200 font-semibold mb-1">Base API</div>
            <div class="text-sm text-white font-mono">localhost:5113</div>
            <div class="text-xs text-yellow-100/70 mt-1">sync/async endpoints</div>
          </div>
        </div>
      </div>

      <!-- Wave decoration -->
      <div class="absolute bottom-0 left-0 right-0">
        <svg class="w-full h-12" viewBox="0 0 1200 120" preserveAspectRatio="none">
          <path d="M0,60 C300,100 900,20 1200,60 L1200,120 L0,120 Z" fill="rgb(249, 250, 251)" opacity="1"/>
        </svg>
      </div>
    </header>

    <!-- Main Content -->
    <main class="relative max-w-7xl mx-auto px-6 -mt-6 pb-16">
      <!-- Mode Selection Card -->
      <section class="bg-white rounded-3xl shadow-2xl border border-gray-200 overflow-hidden">
        <div class="p-8 space-y-6">
          <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-6">
            <div class="space-y-2">
              <div class="flex items-center gap-3">
                <div class="w-1.5 h-8 bg-gradient-to-b from-red-600 to-red-400 rounded-full"></div>
                <div>
                  <p class="text-xs font-bold tracking-[0.25em] text-red-600 uppercase mb-1">
                    Flujos Disponibles
                  </p>
                  <h2 class="text-3xl font-black text-gray-900">
                    Selecciona el modo
                  </h2>
                </div>
              </div>
            </div>

            <!-- Toggle Buttons -->
            <div class="relative bg-gradient-to-r from-gray-100 to-gray-50 p-1.5 rounded-2xl shadow-inner border border-gray-200">
              <div class="flex gap-2">
                <button
                  @click="currentView = 'sync'"
                  :class="[
                    'relative px-6 py-3 rounded-xl font-bold text-sm transition-all duration-300',
                    currentView === 'sync'
                      ? 'bg-gradient-to-r from-red-600 to-red-500 text-white shadow-lg shadow-red-500/50 scale-105'
                      : 'bg-transparent text-gray-600 hover:text-gray-900'
                  ]"
                >
                  <div v-if="currentView === 'sync'" class="absolute inset-0 bg-yellow-400/20 rounded-xl animate-pulse"></div>
                  <span class="relative flex items-center gap-2">
                    <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"/>
                    </svg>
                    Modo Síncrono
                  </span>
                </button>
                
                <button
                  @click="currentView = 'async'"
                  :class="[
                    'relative px-6 py-3 rounded-xl font-bold text-sm transition-all duration-300',
                    currentView === 'async'
                      ? 'bg-gradient-to-r from-red-600 to-red-500 text-white shadow-lg shadow-red-500/50 scale-105'
                      : 'bg-transparent text-gray-600 hover:text-gray-900'
                  ]"
                >
                  <div v-if="currentView === 'async'" class="absolute inset-0 bg-yellow-400/20 rounded-xl animate-pulse"></div>
                  <span class="relative flex items-center gap-2">
                    <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M11.3 1.046A1 1 0 0112 2v5h4a1 1 0 01.82 1.573l-7 10A1 1 0 018 18v-5H4a1 1 0 01-.82-1.573l7-10a1 1 0 011.12-.38z" clip-rule="evenodd"/>
                    </svg>
                    Modo Asíncrono
                  </span>
                </button>
              </div>
            </div>
          </div>

          <!-- Process Content -->
          <div class="border-t-2 border-gray-100 pt-8">
            <div class="transition-all duration-300 ease-out">
              <SyncProcessView v-if="currentView === 'sync'" />
              <AsyncProcessView v-else />
            </div>
          </div>
        </div>
      </section>

      <!-- Checklist Card -->
      <section class="mt-8 bg-white rounded-3xl shadow-xl border border-gray-200 overflow-hidden">
        <div class="p-8">
          <div class="flex items-center gap-4 mb-6">
            <div class="w-14 h-14 bg-gradient-to-br from-red-600 to-red-500 rounded-2xl flex items-center justify-center shadow-lg">
              <svg class="w-8 h-8 text-white" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"/>
              </svg>
            </div>
            <div>
              <h2 class="text-2xl font-black text-gray-900">Checklist</h2>
              <p class="text-gray-600 text-sm">Pasos mínimos para operar el consumo de datos.</p>
            </div>
          </div>

          <div class="grid md:grid-cols-2 gap-4">
            <div 
              v-for="(item, i) in checklistItems" 
              :key="i"
              class="group flex items-start gap-4 p-4 bg-gradient-to-r from-gray-50 to-transparent rounded-xl border border-gray-100 hover:border-red-200 hover:shadow-md transition-all"
            >
              <div class="flex-shrink-0 w-8 h-8 bg-gradient-to-br from-yellow-400 to-yellow-500 rounded-lg flex items-center justify-center text-gray-900 font-black text-sm shadow-sm group-hover:scale-110 transition-transform">
                {{ i + 1 }}
              </div>
              <p class="text-sm text-gray-700 leading-relaxed pt-1">
                {{ item }}
              </p>
            </div>
          </div>
        </div>
      </section>
    </main>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import SyncProcessView from './SyncProcessView.vue'
import AsyncProcessView from './AsyncProcessView.vue'

const currentView = ref('sync')

const checklistItems = [
  'Configurar .env con VITE_API_BASE_URL (default http://localhost:5113).',
  'Servicios: processSyncFile y processAsyncFile usando axios.',
  'Estados: loading, errorMessage, result/acceptedResponse, historial local.',
  'Export JSON de registros válidos en el flujo síncrono.'
]
</script>