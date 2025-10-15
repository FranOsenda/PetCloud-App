// Módulo de autenticación (manejo de token y usuario)
import { API_BASE } from './config.js';
import { authFetch } from './api.js';

const TOKEN_KEY = 'pc_token';

export function saveToken(token) {
  localStorage.setItem(TOKEN_KEY, token);
}

export function getToken() {
  return localStorage.getItem(TOKEN_KEY);
}

export function clearToken() {
  localStorage.removeItem(TOKEN_KEY);
}

export async function login(email, password) {
  const res = await fetch(`${API_BASE}/api/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password })
  });
  if (!res.ok) throw new Error(await res.text() || 'Credenciales inválidas');
  const data = await res.json();
  saveToken(data.token);
  return data;
}

export async function register({ firstName, lastName, email, password, role }) {
  const res = await fetch(`${API_BASE}/api/auth/register`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ firstName, lastName, email, password, role })
  });
  if (!res.ok) throw new Error(await res.text());
  return res.json();
}

export async function getMascotas() {
  return authFetch('/api/mascotas');
}
