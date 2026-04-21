const API_CLIENTES = "http://localhost:5161/api/Cliente/buscar";
const API_PRODUCTOS = "http://localhost:5161/api/Producto/buscar";

const buscarClienteInput = document.getElementById("buscarCliente");
const resultadosClientes = document.getElementById("resultadosClientes");
const clienteSeleccionadoTexto = document.getElementById("clienteSeleccionadoTexto");
const btnCambiarCliente = document.getElementById("btnCambiarCliente");

const buscarProductoInput = document.getElementById("buscarProducto");
const resultadosProductos = document.getElementById("resultadosProductos");
const productoNombre = document.getElementById("productoNombre");
const productoPrecio = document.getElementById("productoPrecio");
const productoStock = document.getElementById("productoStock");
const cantidadProductoInput = document.getElementById("cantidadProducto");
const btnAgregarProducto = document.getElementById("btnAgregarProducto");

const API_FACTURA = "http://localhost:5161/api/Factura";
const btnGuardarFactura = document.getElementById("btnGuardarFactura");

const detalleFacturaBody = document.getElementById("detalleFacturaBody");
const totalFacturaTexto = document.getElementById("totalFactura");

let clienteSeleccionado = null;
let productoSeleccionado = null;
let detalleFactura = [];

// Buscar clientes en tiempo real
buscarClienteInput.addEventListener("input", async () => {
  const texto = buscarClienteInput.value.trim();

  if (texto.length === 0) {
    resultadosClientes.innerHTML = "";
    return;
  }

  try {
    const response = await fetch(`${API_CLIENTES}?texto=${encodeURIComponent(texto)}&pagina=1&tamanioPagina=5`);
    const data = await response.json();

    resultadosClientes.innerHTML = "";

    data.datos.forEach(cliente => {
      const item = document.createElement("div");
      item.className = "resultado-item";
      item.textContent = `${cliente.nombre} ${cliente.apellido} - ${cliente.correo}`;
      item.addEventListener("click", () => {
        clienteSeleccionado = cliente;
        clienteSeleccionadoTexto.textContent = `${cliente.nombre} ${cliente.apellido}`;
        buscarClienteInput.value = `${cliente.nombre} ${cliente.apellido}`;
        resultadosClientes.innerHTML = "";
        buscarClienteInput.disabled = true;
      });
      resultadosClientes.appendChild(item);
    });
  } catch (error) {
    resultadosClientes.innerHTML = "<div class='resultado-item'>Error al buscar clientes</div>";
  }
});

// Cambiar cliente seleccionado
btnCambiarCliente.addEventListener("click", () => {
  clienteSeleccionado = null;
  clienteSeleccionadoTexto.textContent = "Ninguno";
  buscarClienteInput.value = "";
  buscarClienteInput.disabled = false;
  resultadosClientes.innerHTML = "";
});

// Buscar productos en tiempo real
buscarProductoInput.addEventListener("input", async () => {
  const texto = buscarProductoInput.value.trim();

  if (texto.length === 0) {
    resultadosProductos.innerHTML = "";
    return;
  }

  try {
    const response = await fetch(`${API_PRODUCTOS}?texto=${encodeURIComponent(texto)}&pagina=1&tamanioPagina=5`);
    const data = await response.json();

    resultadosProductos.innerHTML = "";

    data.datos.forEach(producto => {
      const item = document.createElement("div");
      item.className = "resultado-item";
      item.textContent = `${producto.nombre} - $${Number(producto.precio).toFixed(2)} - Stock: ${producto.stock}`;
      item.addEventListener("click", () => {
        productoSeleccionado = producto;
        productoNombre.textContent = producto.nombre;
        productoPrecio.textContent = `$${Number(producto.precio).toFixed(2)}`;
        productoStock.textContent = producto.stock;
        buscarProductoInput.value = producto.nombre;
        resultadosProductos.innerHTML = "";
      });
      resultadosProductos.appendChild(item);
    });
  } catch (error) {
    resultadosProductos.innerHTML = "<div class='resultado-item'>Error al buscar productos</div>";
  }
});

// Agregar producto al detalle
btnAgregarProducto.addEventListener("click", () => {
  if (!productoSeleccionado) {
    mostrarToast("Selecciona un producto", "warning");
    return;
  }

  const cantidad = parseInt(cantidadProductoInput.value);

  if (!cantidad || cantidad <= 0) {
    mostrarToast("Ingresa una cantidad válida", "warning");
    return;
  }

  if (cantidad > productoSeleccionado.stock) {
    mostrarToast("La cantidad no puede superar el stock disponible", "error");
    return;
  }

  const existente = detalleFactura.find(d => d.idProducto === productoSeleccionado.idProducto);

  if (existente) {
    if (existente.cantidad + cantidad > productoSeleccionado.stock) {
      mostrarToast("La cantidad total supera el stock disponible", "error");
      return;
    }

    existente.cantidad += cantidad;
    existente.totalLinea = existente.cantidad * existente.precioUnitario;
  } else {
    detalleFactura.push({
      idProducto: productoSeleccionado.idProducto,
      nombre: productoSeleccionado.nombre,
      precioUnitario: productoSeleccionado.precio,
      cantidad: cantidad,
      totalLinea: productoSeleccionado.precio * cantidad
    });
  }

  renderDetalleFactura();

  cantidadProductoInput.value = "";
  productoSeleccionado = null;
  productoNombre.textContent = "Ninguno";
  productoPrecio.textContent = "$0.00";
  productoStock.textContent = "0";
  buscarProductoInput.value = "";
});

// Render tabla detalle
function renderDetalleFactura() {
  detalleFacturaBody.innerHTML = "";

  let totalGeneral = 0;

  detalleFactura.forEach((item, index) => {
    totalGeneral += item.totalLinea;

    const fila = document.createElement("tr");
    fila.innerHTML = `
      <td>${item.idProducto}</td>
      <td>${item.nombre}</td>
      <td>$${Number(item.precioUnitario).toFixed(2)}</td>
      <td>${item.cantidad}</td>
      <td>$${Number(item.totalLinea).toFixed(2)}</td>
      <td><button onclick="eliminarDetalle(${index})">Quitar</button></td>
    `;
    detalleFacturaBody.appendChild(fila);
  });

  totalFacturaTexto.textContent = `$${totalGeneral.toFixed(2)}`;
}

window.eliminarDetalle = function(index) {
  detalleFactura.splice(index, 1);
  renderDetalleFactura();
};

btnGuardarFactura.addEventListener("click", async () => {
  if (!clienteSeleccionado) {
    mostrarToast("Selecciona un cliente", "warning");
    return;
  }

  if (detalleFactura.length === 0) {
    mostrarToast("Agrega al menos un producto", "warning");
    return;
  }

  const factura = {
    idCliente: clienteSeleccionado.idCliente,
    detalles: detalleFactura.map(d => ({
      idProducto: d.idProducto,
      cantidad: d.cantidad
    }))
  };

  try {
    const response = await fetch(API_FACTURA, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(factura)
    });

    if (!response.ok) {
      throw new Error("Error al guardar la factura");
    }

    const data = await response.json();

    mostrarToast("Factura guardada correctamente", "success");

    if (data.numeroFactura) {
      window.open(`http://localhost:5161/api/Factura/pdf/${data.numeroFactura}`, "_blank");
    }

    // limpiar todo
    detalleFactura = [];
    renderDetalleFactura();
    clienteSeleccionado = null;
    clienteSeleccionadoTexto.textContent = "Ninguno";
    buscarClienteInput.value = "";
    buscarClienteInput.disabled = false;
    resultadosClientes.innerHTML = "";
  } catch (error) {
    mostrarToast(error.message, "error");
  }
});