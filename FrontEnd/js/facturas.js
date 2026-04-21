const API = "http://localhost:5161/api/Factura";

const tabla = document.getElementById("tablaFacturas");

// 🔹 Buscar por número
async function buscarPorNumero() {
  const numero = document.getElementById("buscarNumero").value;

  if (!numero) {
    mostrarToast("Ingresa un número de factura", "warning");
    return;
  }

  try {
    const res = await fetch(`${API}/${numero}`);
    const data = await res.json();

    tabla.innerHTML = "";

    agregarFila(data);
  } catch {
    mostrarToast("Factura no encontrada", "error");
  }
}

// 🔹 Buscar por fecha
async function buscarPorFecha() {
  const fecha = document.getElementById("buscarFecha").value;

  if (!fecha) {
    mostrarToast("Selecciona una fecha", "warning");
    return;
  }

  try {
    const res = await fetch(`${API}/fecha?fecha=${fecha}&pagina=1&tamanioPagina=10`);
    const data = await res.json();

    tabla.innerHTML = "";

    data.datos.forEach(f => agregarFila(f));
  } catch {
    mostrarToast("Error al buscar facturas", "error");
  }
}

// 🔹 agregar fila a la tabla
function agregarFila(factura) {
  const fila = document.createElement("tr");

  fila.innerHTML = `
    <td>${factura.numeroFactura}</td>
    <td>${factura.fecha}</td>
    <td>${factura.cliente}</td>
    <td>$${factura.total}</td>
    <td>
      <button onclick="verPDF(${factura.numeroFactura})">
        Imprimir
      </button>
    </td>
  `;

  tabla.appendChild(fila);
}

// 🔹 abrir PDF
function verPDF(numero) {
  window.open(`http://localhost:5161/api/Factura/pdf/${numero}`, "_blank");
}