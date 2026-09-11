document.addEventListener("DOMContentLoaded",async()=>{
  if(!requireLogin("/favorites.html")) return;
  const box=document.getElementById("favorites");
  try{
    const data=await api("/favorites");
    if(!data.length){box.innerHTML="<div class='panel'>No saved flights yet. Tap the heart on a flight to save it.</div>";return}
    box.innerHTML=data.map(x=>{const f=x.flight;return `<article class="flight-card"><img class="flight-img" src="${f.imageUrl}" alt=""><div><div class="meta">${f.airline} • ${f.flightNumber}</div><div class="route"><strong>${f.originCode}</strong><div class="route-line"></div><strong>${f.destinationCode}</strong></div><div class="meta">${new Date(f.departureUtc).toLocaleString()} • ${f.availableSeats} seats</div><div class="card-actions"><button class="icon-btn" onclick="removeFav(${f.id})">Remove</button><button class="btn btn-primary" onclick="selectFlight(${f.id})">Book</button></div></div><div class="price">₹${Number(f.price).toLocaleString("en-IN")}<small>per passenger</small></div></article>`}).join("");
  }catch(e){box.innerHTML=`<div class="panel">${e.message}</div>`}
});
async function removeFav(id){await api("/favorites/"+id,{method:"DELETE"});location.reload()}
function selectFlight(id){location.href="/checkout.html?flight="+id}
