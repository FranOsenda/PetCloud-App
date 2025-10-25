import { authFetch } from './api.js';

const modal = () => document.getElementById('modal');
const get = id => document.getElementById(id);

export async function listMascotas(container) {
  container.innerHTML = '<p>Cargando...</p>';
  try {
    const mascotas = await authFetch('/api/mascotas');
    if (!mascotas || mascotas.length === 0) {
      container.innerHTML = '<p class="text-gray-500">No hay mascotas. Crea una nueva.</p>';
      return;
    }
    container.innerHTML = mascotas.map(m => (`
      <div class="p-4 bg-white dark:bg-gray-800 rounded shadow flex items-center justify-between">
        <div>
          <div class="font-semibold">${m.nombre} <span class="text-sm text-gray-400">(${m.especie})</span></div>
          <div class="text-sm text-gray-500">Raza: ${m.raza} · Sexo: ${m.sexo || '-'} · Peso: ${m.peso || '-'} kg</div>
        </div>
        <div class="flex gap-2">
          <button data-id="${m.id}" class="btn-edit px-3 py-1 bg-yellow-400 rounded">Editar</button>
          <button data-id="${m.id}" class="btn-del px-3 py-1 bg-red-500 text-white rounded">Eliminar</button>
        </div>
      </div>
    `)).join('');

    container.querySelectorAll('.btn-edit').forEach(b => b.addEventListener('click', (ev) => openEditModal(ev.target.dataset.id)));
    container.querySelectorAll('.btn-del').forEach(b => b.addEventListener('click', (ev) => deleteMascota(ev.target.dataset.id, container)));
  } catch (e) {
    container.innerHTML = '<p class="text-red-500">Error al cargar mascotas. Asegúrate de estar logueado.</p>';
  }
}

export function openCreateModal() {
  openModal();
}

export async function openEditModal(id) {
  try {
    const m = await authFetch(`/api/mascotas/${id}`);
    openModal(m);
  } catch (e) {
    alert('No se pudo cargar la mascota');
  }
}

function openModal(data = null) {
  const md = modal();
  md.classList.remove('hidden');
  md.classList.add('flex');
  get('modal-title').textContent = data ? 'Editar mascota' : 'Nueva mascota';
  get('m-nombre').value = data?.nombre || '';
  get('m-especie').value = data?.especie || '';
  get('m-raza').value = data?.raza || '';
  get('m-sexo').value = data?.sexo || '';
  get('m-fecha').value = data?.fechaNacimiento ? (new Date(data.fechaNacimiento)).toISOString().substr(0,10) : '';
  get('m-peso').value = data?.peso ?? '';

  get('modal-cancel').onclick = () => closeModal();
  get('modal-save').onclick = async () => {
    const payload = {
      nombre: get('m-nombre').value,
      especie: get('m-especie').value,
      raza: get('m-raza').value,
      sexo: get('m-sexo').value,
      fechaNacimiento: get('m-fecha').value || null,
      peso: get('m-peso').value ? parseFloat(get('m-peso').value) : null
    };
    try {
      if (data) {
        await authFetch(`/api/mascotas/${data.id}`, { method: 'PUT', body: JSON.stringify(payload) });
      } else {
        await authFetch('/api/mascotas', { method: 'POST', body: JSON.stringify(payload) });
      }
      closeModal();
      // refresh list
      const container = document.getElementById('mascotas-list');
      await listMascotas(container);
    } catch (err) {
      alert('Error guardando mascota: ' + (err.message || err));
    }
  };
}

function closeModal() {
  const md = modal();
  md.classList.add('hidden');
  md.classList.remove('flex');
}

async function deleteMascota(id, container) {
  if (!confirm('Eliminar mascota?')) return;
  try {
    await authFetch(`/api/mascotas/${id}`, { method: 'DELETE' });
    await listMascotas(container);
  } catch (e) {
    alert('Error eliminando mascota');
  }
}
