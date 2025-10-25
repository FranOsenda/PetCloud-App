// utilidades de UI: tema y header con selector de mascota
import { authFetch } from './api.js';

export function initThemeToggle(buttonId = 'theme-toggle') {
  const btn = document.getElementById(buttonId);
  if (!btn) return;
  const saved = localStorage.getItem('theme') || 'light';
  if (saved === 'dark') document.documentElement.classList.add('dark');
  btn.textContent = saved === 'dark' ? 'Tema claro' : 'Tema oscuro';
  btn.onclick = () => {
    document.documentElement.classList.toggle('dark');
    const isDark = document.documentElement.classList.contains('dark');
    localStorage.setItem('theme', isDark ? 'dark' : 'light');
    btn.textContent = isDark ? 'Tema claro' : 'Tema oscuro';
  };
}

export async function renderHeader(containerId = 'app-header') {
  const el = document.getElementById(containerId);
  if (!el) return;
  // Cargar mascotas para el selector
  let mascotas = [];
  try {
    mascotas = await authFetch('/api/mascotas');
  } catch (e) { /* no logueado */ }

  const selectedId = localStorage.getItem('mascotaId');
  const options = mascotas.map(m => `<option value="${m.id}" ${String(m.id)===selectedId?'selected':''}>${m.nombre}</option>`).join('');

  el.innerHTML = `
    <div class="flex items-center justify-between p-4 bg-white dark:bg-gray-900 shadow">
      <a href="dashboard.html" class="text-xl font-bold text-blue-600">PetCloud</a>
      <div class="flex items-center gap-3">
        <select id="sel-mascota" class="border rounded p-2 dark:bg-gray-800 dark:text-gray-100">
          <option value="">Seleccionar mascota</option>
          ${options}
        </select>
        <button id="theme-toggle" class="px-3 py-2 rounded bg-gray-200 dark:bg-gray-700 dark:text-gray-100">Tema oscuro</button>
      </div>
    </div>`;

  const sel = document.getElementById('sel-mascota');
  sel?.addEventListener('change', () => {
    if (sel.value) localStorage.setItem('mascotaId', sel.value);
  });
}
