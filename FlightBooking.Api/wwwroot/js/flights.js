function money(n){return "₹"+Number(n).toLocaleString("en-IN")}
function fmt(dt){return new Date(dt).toLocaleTimeString([], {hour:"2-digit",minute:"2-digit"})}
async function loadFlights(){
  const o=document.getElementById("origin").value.trim();
  const d=document.getElementById("destination").value.trim();
  const list=document.getElementById("flightList");
  list.innerHTML="<div class='panel'>Searching…</div>";
  try{
    const data=await api("/flights?origin="+encodeURIComponent(o)+"&destination="+encodeURIComponent(d));
    if(!data.length){list.innerHTML="<div class='panel'>No flights found. Try HYD → DEL, BLR → GOI or another route.</div>";return}
    list.innerHTML=data.map(f=>`
      <article class="flight-card">
        <img class="flight-img" src="${f.imageUrl}" alt="${f.destination}">
        <div>
          <div class="meta">${f.airline} • ${f.flightNumber} • ${f.availableSeats} seats left</div>
          <div class="route"><strong>${f.originCode}</strong><div class="route-line"></div><strong>${f.destinationCode}</strong></div>
          <div class="meta">${fmt(f.departureUtc)} — ${fmt(f.arrivalUtc)} • Direct • Economy</div>
          <div class="card-actions">
            <button class="icon-btn" onclick="favorite(${f.id})">♡</button>
            <button class="btn btn-primary" onclick="selectFlight(${f.id})">Select flight</button>
          </div>
        </div>
        <div class="price">${money(f.price)}<small>per passenger</small></div>
      </article>`).join("");
  }catch(e){list.innerHTML=`<div class="panel">${e.message}</div>`}
}
function selectFlight(id){
  if(!requireLogin("/checkout.html?flight="+id)) return;
  location.href="/checkout.html?flight="+id;
}
async function favorite(id){
  if(!requireLogin("/flights.html")) return;
  try{await api("/favorites/"+id,{method:"POST"});alert("Saved to favorites.")}catch(e){alert(e.message)}
}
document.addEventListener("DOMContentLoaded",()=>{
  const q=new URLSearchParams(location.search);
  document.getElementById("origin").value=q.get("from")||"";
  document.getElementById("destination").value=q.get("to")||"";
  loadFlights();
});
