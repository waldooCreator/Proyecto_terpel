import axios from 'axios'

// Cliente axios básico apuntando al backend local (configurable vía .env)
const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5113',
  headers: {
    'Content-Type': 'application/json'
  }
})

const normalizeError = (error) => {
  const message = error?.response?.data?.message || error.message || 'Error de red'
  const wrapped = new Error(message)
  wrapped.response = error?.response
  return wrapped
}

export const processSyncFile = async (dynamicUrl = 'real', authType = 'bearer', callbackUrl) => {
  try {
    const payload = { DynamicUrl: dynamicUrl, AuthType: authType, CallbackUrl: callbackUrl }
    const { data } = await api.post('/api/terpel/ventas/sync', payload)
    return data
  } catch (error) {
    throw normalizeError(error)
  }
}

export const processAsyncFile = async (dynamicUrl = 'real', authType = 'OAuth', callbackUrl) => {
  try {
    const payload = { DynamicUrl: dynamicUrl, AuthType: authType, CallbackUrl: callbackUrl }
    const { data } = await api.post('/api/terpel/ventas/async', payload)
    return data
  } catch (error) {
    throw normalizeError(error)
  }
}

export const syncToPhp = async (payload) => {
  try {
    const { data } = await api.post('/api/terpel/ventas/sync-php', payload)
    return data
  } catch (error) {
    throw normalizeError(error)
  }
}

export const getConsolidadosEDS = async () => {
  try {
    const { data } = await api.get('/api/v1/eds/consolidados-eds')
    return data
  } catch (error) {
    throw normalizeError(error)
  }
}

export default api
