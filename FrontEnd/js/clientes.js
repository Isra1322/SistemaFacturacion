const API_URL = "http://localhost:5161/api/Cliente";

const clienteForm = document.getElementById("clienteForm");
const buscarCliente = document.getElementById("buscarCliente");
const clientesBody = document.getElementById("clientesBody");
const btnAnterior = document.getElementById("btnAnterior");
const btnSiguiente = document.getElementById("btnSiguiente");
const paginaActualTexto = document.getElementById("paginaActual");

let paginaActual = 1;
const tamanioPagina = 10;
let totalPaginas = 1;
let textoBusqueda = "";

// 🔹 SOLO NÚMEROS EN TELÉFONO
document.getElementById("telefono").addEventListener("input", (e) => {
  e.target.value = e.target.value.replace(/\D/g, "");
});

document.addEventListener("DOMContentLoaded", () => {
  cargarClientes();
});

clienteForm.addEventListener("submit", async (e) => {
  e.preventDefault();

  const nombre = document.getElementById("nombre").value.trim();
  const apellido = document.getElementById("apellido").value.trim();
  const direccion = document.getElementById("direccion").value.trim();
  const telefono = document.getElementById("telefono").value.trim();
  const correo = document.getElementById("correo").value.trim();

  // 🔹 VALIDACIONES
  if (!nombre || !apellido || !direccion || !telefono || !correo) {
    mostrarToast("Todos los campos son obligatorios", "warning");
    return;
  }

  // 🔹 VALIDAR TELÉFONO ECUADOR
  const telefonoRegex = /^09\d{8}$/;
  if (!telefonoRegex.test(telefono)) {
    mostrarToast("Ingrese un número válido (Ej: 0991234567)", "warning");
    return;
  }

  const cliente = {
    nombre,
    apellido,
    direccion,
    telefono,
    correo
  };

  try {
    const response = await fetch(API_URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(cliente)
    });

    if (!response.ok) {
      let mensajeError = "No se pudo guardar el cliente";

      try {
        const errorData = await response.json();
        mensajeError = errorData.mensaje || errorData.error || mensajeError;
      } catch {
        // fallback
      }

      throw new Error(mensajeError);
    }

    clienteForm.reset();
    paginaActual = 1;
    cargarClientes();
    mostrarToast("Cliente guardado correctamente", "success");

  } catch (error) {
    mostrarToast(error.message, "error");
  }
});

buscarCliente.addEventListener("input", () => {
  textoBusqueda = buscarCliente.value.trim();
  paginaActual = 1;
  cargarClientes();
});

btnAnterior.addEventListener("click", () => {
  if (paginaActual > 1) {
    paginaActual--;
    cargarClientes();
  }
});

btnSiguiente.addEventListener("click", () => {
  if (paginaActual < totalPaginas) {
    paginaActual++;
    cargarClientes();
  }
});

async function cargarClientes() {
  try {
    const response = await fetch(
      `${API_URL}/buscar?texto=${encodeURIComponent(textoBusqueda)}&pagina=${paginaActual}&tamanioPagina=${tamanioPagina}`
    );

    if (!response.ok) {
      throw new Error("No se pudieron cargar los clientes");
    }

    const data = await response.json();

    clientesBody.innerHTML = "";
    totalPaginas = data.totalPaginas || 1;
    paginaActualTexto.textContent = `Página ${paginaActual} de ${totalPaginas}`;

    if (!data.datos || data.datos.length === 0) {
      clientesBody.innerHTML = `
        <tr>
          <td colspan="6" class="empty-state">No se encontraron clientes</td>
        </tr>
      `;

      btnAnterior.disabled = true;
      btnSiguiente.disabled = true;
      return;
    }

    data.datos.forEach(cliente => {
      const fila = `
        <tr>
          <td>${cliente.idCliente}</td>
          <td>${cliente.nombre}</td>
          <td>${cliente.apellido}</td>
          <td>${cliente.direccion}</td>
          <td>${cliente.telefono}</td>
          <td>${cliente.correo}</td>
        </tr>
      `;
      clientesBody.innerHTML += fila;
    });

    btnAnterior.disabled = paginaActual === 1;
    btnSiguiente.disabled = paginaActual >= totalPaginas;

  } catch (error) {
    clientesBody.innerHTML = `
      <tr>
        <td colspan="6" class="empty-state">Error al cargar clientes</td>
      </tr>
    `;
  }
}