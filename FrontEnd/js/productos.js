const API_URL = "http://localhost:5161/api/Producto";

const productoForm = document.getElementById("productoForm");
const buscarProducto = document.getElementById("buscarProducto");
const productosBody = document.getElementById("productosBody");
const btnAnterior = document.getElementById("btnAnterior");
const btnSiguiente = document.getElementById("btnSiguiente");
const paginaActualTexto = document.getElementById("paginaActual");

let paginaActual = 1;
const tamanioPagina = 10;
let totalPaginas = 1;
let textoBusqueda = "";

document.addEventListener("DOMContentLoaded", () => {
  cargarProductos();
});

productoForm.addEventListener("submit", async (e) => {
  e.preventDefault();

  const nombre = document.getElementById("nombre").value.trim();
  const precio = parseFloat(document.getElementById("precio").value);
  const stock = parseInt(document.getElementById("stock").value);

  // Validaciones frontend
  if (!nombre) {
    mostrarToast("El nombre del producto es obligatorio", "warning");
    return;
  }

  if (isNaN(precio)) {
    mostrarToast("Ingresa un precio válido", "warning");
    return;
  }

  if (precio <= 0) {
    mostrarToast("El precio debe ser mayor a 0", "warning");
    return;
  }

  if (isNaN(stock)) {
    mostrarToast("Ingresa un stock válido", "warning");
    return;
  }

  if (stock < 0) {
    mostrarToast("El stock no puede ser negativo", "warning");
    return;
  }

  const producto = {
    nombre,
    precio,
    stock
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
  let mensajeError = "No se pudo guardar el producto";

  try {
    const errorData = await response.json();
    mensajeError = errorData.error || errorData.mensaje || mensajeError;
  } catch {
    mensajeError = "No se pudo leer el detalle del error";
  }

  throw new Error(mensajeError);
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

    if (!data.datos || data.datos.length === 0) {
      productosBody.innerHTML = `
        <tr>
          <td colspan="4" class="empty-state">No se encontraron productos</td>
        </tr>
      `;
      btnAnterior.disabled = true;
      btnSiguiente.disabled = true;
      return;
    }

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
        <td colspan="4" class="empty-state">Error al cargar productos</td>
      </tr>
    `;
  }
}