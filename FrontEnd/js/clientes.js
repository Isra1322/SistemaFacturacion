const API_URL = "http://localhost:5161/api/Cliente";

const clienteForm = document.getElementById("clienteForm");
const buscarCliente = document.getElementById("buscarCliente");
const clientesBody = document.getElementById("clientesBody");
const btnAnterior = document.getElementById("btnAnterior");
const btnSiguiente = document.getElementById("btnSiguiente");
const paginaActualTexto = document.getElementById("paginaActual");

let paginaActual = 1;
const tamanioPagina = 5;
let totalPaginas = 1;
let textoBusqueda = "";

document.addEventListener("DOMContentLoaded", () => {
  cargarClientes();
});

clienteForm.addEventListener("submit", async (e) => {
  e.preventDefault();

  const cliente = {
    nombre: document.getElementById("nombre").value.trim(),
    apellido: document.getElementById("apellido").value.trim(),
    direccion: document.getElementById("direccion").value.trim(),
    telefono: document.getElementById("telefono").value.trim(),
    correo: document.getElementById("correo").value.trim()
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
    const errorData = await response.json();
    throw new Error(errorData.mensaje || "No se pudo guardar el cliente");
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
        <td colspan="6">Error al cargar clientes</td>
      </tr>
    `;
  }
}