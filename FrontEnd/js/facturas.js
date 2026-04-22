const API = "http://localhost:5161/api/Factura";

const tabla = document.getElementById("tablaFacturas");

// Buscar por número
async function buscarPorNumero() {
  const numero = document.getElementById("buscarNumero").value;

  if (!numero) {
    mostrarToast("Ingresa un número de factura", "warning");
    return;
  }

  try {
    const res = await fetch(`${API}/${numero}`);

    if (!res.ok) {
      throw new Error("Factura no encontrada");
    }

    const data = await res.json();

    tabla.innerHTML = "";
    agregarFila(data);
  } catch (error) {
    mostrarToast(error.message, "error");
  }
}

// Buscar por fecha
async function buscarPorFecha() {
  const fecha = document.getElementById("buscarFecha").value;

  if (!fecha) {
    mostrarToast("Selecciona una fecha", "warning");
    return;
  }

  try {
    const res = await fetch(`${API}/fecha?fecha=${fecha}&pagina=1&tamanioPagina=10`);

    if (!res.ok) {
      throw new Error("Error al buscar facturas");
    }

    const data = await res.json();

    tabla.innerHTML = "";

    if (!data.datos || data.datos.length === 0) {
      tabla.innerHTML = `
        <tr>
          <td colspan="5" class="empty-state">No se encontraron facturas para esa fecha</td>
        </tr>
      `;
      return;
    }

    data.datos.forEach(f => agregarFila(f));
  } catch (error) {
    mostrarToast(error.message, "error");
  }
}

// Agregar fila a la tabla
function agregarFila(factura) {
  const fila = document.createElement("tr");

  fila.innerHTML = `
    <td>${factura.numeroFactura}</td>
    <td>${formatearFecha(factura.fecha)}</td>
    <td>${factura.cliente}</td>
    <td>$${Number(factura.total).toFixed(2)}</td>
    <td>
      <div class="action-row" style="justify-content:center;">
        <button class="small-button" onclick="verFactura(${factura.numeroFactura})">Ver</button>
        <button class="small-button" onclick="verPDF(${factura.numeroFactura})">Imprimir</button>
      </div>
    </td>
  `;

  tabla.appendChild(fila);
}

// Ver factura
function verFactura(numero) {
  window.location.href = `./ver-factura.html?numero=${numero}`;
}

// Abrir PDF
function verPDF(numero) {
  window.open(`http://localhost:5161/api/Factura/pdf/${numero}`, "_blank");
}

// Formatear fecha
function formatearFecha(fechaTexto) {
  const fecha = new Date(fechaTexto);

  if (isNaN(fecha)) return fechaTexto;

  return fecha.toLocaleString("es-EC", {
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit"
  });
}