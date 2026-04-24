const API_FACTURA = "http://localhost:5161/api/Factura";

const numeroFacturaTexto = document.getElementById("numeroFactura");
const fechaFacturaTexto = document.getElementById("fechaFactura");
const clienteFacturaTexto = document.getElementById("clienteFactura");
const detalleFacturaBody = document.getElementById("detalleFacturaBody");
const subtotalFacturaTexto = document.getElementById("subtotalFactura");
const ivaFacturaTexto = document.getElementById("ivaFactura");
const totalFacturaTexto = document.getElementById("totalFactura");
const btnImprimirFactura = document.getElementById("btnImprimirFactura");

const params = new URLSearchParams(window.location.search);
const numeroFactura = params.get("numero");

document.addEventListener("DOMContentLoaded", () => {
  if (!numeroFactura) {
    mostrarToast("No se recibió un número de factura", "error");
    return;
  }

  cargarFactura(numeroFactura);
});

async function cargarFactura(numero) {
  try {
    const response = await fetch(`${API_FACTURA}/${numero}`);

    if (!response.ok) {
      throw new Error("Factura no encontrada");
    }

    const factura = await response.json();

    numeroFacturaTexto.textContent = factura.numeroFactura;
    fechaFacturaTexto.textContent = factura.fecha;
    clienteFacturaTexto.textContent = factura.cliente;
    subtotalFacturaTexto.textContent = `$${Number(factura.subtotal).toFixed(2)}`;
    ivaFacturaTexto.textContent = `$${Number(factura.iva).toFixed(2)}`;
    totalFacturaTexto.textContent = `$${Number(factura.total).toFixed(2)}`;

    detalleFacturaBody.innerHTML = "";

    factura.detalles.forEach(detalle => {
      const fila = document.createElement("tr");
      fila.innerHTML = `
        <td>${detalle.producto}</td>
        <td>${detalle.cantidad}</td>
        <td>$${Number(detalle.precioUnitario).toFixed(2)}</td>
        <td>$${Number(detalle.totalLinea).toFixed(2)}</td>
      `;
      detalleFacturaBody.appendChild(fila);
    });

  } catch (error) {
    mostrarToast(error.message, "error");
  }
}

btnImprimirFactura.addEventListener("click", async () => {
  if (!numeroFactura) return;

  try {
    const response = await fetch(`${API_FACTURA}/pdf/${numeroFactura}`);

    if (!response.ok) {
      throw new Error("No se pudo generar el PDF");
    }

    mostrarToast("Factura guardada correctamente", "success");
  } catch (error) {
    mostrarToast(error.message, "error");
  }
});