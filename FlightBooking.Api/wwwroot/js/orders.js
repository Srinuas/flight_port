document.addEventListener("DOMContentLoaded",async()=>{
  if(!requireLogin("/orders.html")) return;
  const box=document.getElementById("orders");
  try{
    const orders=await api("/orders");
    if(!orders.length){box.innerHTML="<div class='panel'>No bookings yet. Explore flights and plan your next trip.</div>";return}
    box.innerHTML=orders.map(o=>{
      const i=o.items[0],f=i.flight;
      return `<article class="order-card"><div class="order-head"><div><span class="pill">${o.status}</span><h2>${o.orderNumber}</h2><p class="muted">${new Date(o.createdAtUtc).toLocaleString()}</p></div><strong>₹${Number(o.totalAmount).toLocaleString("en-IN")}</strong></div><hr><p><b>${f.originCode} → ${f.destinationCode}</b> • ${f.airline} • ${i.passengerName} • ${i.seatCount} passenger(s)</p><p class="muted">Payment: ${o.payment?.method||"—"} • Ref: ${o.payment?.reference||"—"}</p></article>`
    }).join("");
  }catch(e){box.innerHTML=`<div class="panel">${e.message}</div>`}
});
