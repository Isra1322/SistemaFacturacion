const API_URL = "http://localhost:5161/api/Producto";

const productoForm = document.getElementById("productoForm");
const buscarProducto = document.getElementById("buscarProducto");
const productosBody = document.getElementById("productosBody");
const btnAnterior = document.getElementById("btnAnterior");
const btnSiguiente = document.getElementById("btnSiguiente");
const paginaActualTexto = document.getElementById("paginaActual");

let paginaActual = 1;
const tamanioPagina = 5;
let totalPaginas = 1;
let textoBusqueda = "";

document.addEventListener("DOMContentLoaded", () => {
  cargarProductos();
});

productoForm.addEventListener("submit", async (e) => {
  e.preventDefault();

  const producto = {
    nombre: document.getElementById("nombre").value.trim(),
    precio: parseFloat(document.getElementById("precio").value),
    stock: parseInt(document.getElementById("stock").value)
  };

  try {
    const response = await fetch(API_URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(producto)
    });

    if (!response.ok) {
      throw new Error("No se pudo guardar el producto");
    }

    productoForm.reset();
    paginaActual = 1;
    cargarProductos();
    mostrarToast("Producto guardado correctamente", "success");
  } catch (error) {
    mostrarToast(error.message, "error");
  }
});

buscarProducto.addEventListener("input", () => {
  textoBusqueda = buscarProducto.value.trim();
  paginaActual = 1;
  cargarProductos();
});

btnAnterior.addEventListener("click", () => {
  if (paginaActual > 1) {
    paginaActual--;
    cargarProductos();
  }
});

btnSiguiente.addEventListener("click", () => {
  if (paginaActual < totalPaginas) {
    paginaActual++;
    cargarProductos();
  }
});

async function cargarProductos() {
  try {
    const response = await fetch(
      `${API_URL}/buscar?texto=${encodeURIComponent(textoBusqueda)}&pagina=${paginaActual}&tamanioPagina=${tamanioPagina}`
    );

    if (!response.ok) {
      throw new Error("No se pudieron cargar los productos");
    }

    const data = await response.json();

    productosBody.innerHTML = "";
    totalPaginas = data.totalPaginas || 1;
    paginaActualTexto.textContent = `Página ${paginaActual} de ${totalPaginas}`;

    data.datos.forEach(producto => {
      const fila = `
        <tr>
          <td>${producto.idProducto}</td>
          <td>${producto.nombre}</td>
          <td>$${Number(producto.precio).toFixed(2)}</td>
          <td>${producto.stock}</td>
        </tr>
      `;
      productosBody.innerHTML += fila;
    });

    btnAnterior.disabled = paginaActual === 1;
    btnSiguiente.disabled = paginaActual >= totalPaginas;
  } catch (error) {
    productosBody.innerHTML = `
      <tr>
        <td colspan="4">Error al cargar productos</td>
      </tr>
    `;
  }
}