let captchaId="",method="UPI",flight=null;
async function checkoutCaptcha(){const d=await api("/auth/captcha");captchaId=d.id;document.getElementById("captchaQuestion").textContent="Security check: "+d.question}
function renderPayment(){
  const box=document.getElementById("paymentFields");
  if(method==="UPI") box.innerHTML=`<label>UPI ID<input id="upi" placeholder="name@bank"></label>`;
  else if(method==="CARD") box.innerHTML=`<label>Card number<input id="cardNumber" inputmode="numeric" maxlength="19" placeholder="1111 2222 3333 4444"></label><div class="two-col"><label>Expiry<input placeholder="MM/YY"></label><label>CVV<input type="password" maxlength="3" placeholder="•••"></label></div>`;
  else box.innerHTML=`<label>Bank<select><option>HDFC Bank</option><option>ICICI Bank</option><option>SBI</option><option>Axis Bank</option></select></label>`;
}
document.addEventListener("DOMContentLoaded",async()=>{
  if(!requireLogin(location.pathname+location.search)) return;
  const id=new URLSearchParams(location.search).get("flight");
  if(!id){document.getElementById("flightSummary").textContent="No flight selected.";return}
  try{
    flight=await api("/flights/"+id);
    document.getElementById("flightSummary").innerHTML=`<span class="eyebrow">${flight.airline}</span><h2>${flight.originCode} → ${flight.destinationCode}</h2><p>${flight.origin} to ${flight.destination}</p><p>${new Date(flight.departureUtc).toLocaleString()}</p><h2>₹${Number(flight.price).toLocaleString("en-IN")} <small>/ passenger</small></h2><img class="flight-img" style="width:100%;height:220px" src="${flight.imageUrl}" alt="">`;
    document.querySelectorAll(".payment-tabs button").forEach(b=>b.onclick=()=>{document.querySelectorAll(".payment-tabs button").forEach(x=>x.classList.remove("active"));b.classList.add("active");method=b.dataset.method;renderPayment()});
    renderPayment();await checkoutCaptcha();
  }catch(e){document.getElementById("checkoutMessage").textContent=e.message}
  document.getElementById("payBtn").onclick=async()=>{
    const msg=document.getElementById("checkoutMessage");msg.className="message";msg.textContent="";
    try{
      let lastFour="";
      if(method==="CARD"){const digits=(document.getElementById("cardNumber").value||"").replace(/\D/g,"");if(digits.length<12)throw new Error("Enter a valid demo card number.");lastFour=digits.slice(-4)}
      const d=await api("/orders",{method:"POST",body:JSON.stringify({
        flightId:flight.id,passengerName:document.getElementById("passengerName").value,
        seatCount:Number(document.getElementById("seatCount").value),paymentMethod:method,
        cardLastFour:lastFour,captchaId,captchaAnswer:document.getElementById("captchaAnswer").value
      })});
      msg.className="message success";msg.textContent="Booking confirmed: "+d;
      setTimeout(()=>location.href="/orders.html",1200);
    }catch(e){msg.textContent=e.message;await checkoutCaptcha()}
  };
});
