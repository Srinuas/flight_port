const API = "/api";
const token = () => localStorage.getItem("flyora_token");
const user = () => JSON.parse(localStorage.getItem("flyora_user") || "null");

function headers(json=true){
  const h = {};
  if(json) h["Content-Type"]="application/json";
  if(token()) h["Authorization"]="Bearer "+token();
  return h;
}
async function api(path, options={}){
  options.headers = {...headers(options.body !== undefined), ...(options.headers||{})};
  const res = await fetch(API+path, options);
  const text = await res.text();
  let data;
  try{data=JSON.parse(text)}catch{data=text}
  if(!res.ok) throw new Error(data?.message || data?.title || data || "Request failed");
  return data;
}
function requireLogin(next="/flights.html"){
  if(!token()){ location.href="/login.html?next="+encodeURIComponent(next); return false; }
  return true;
}
function saveAuth(data){
  localStorage.setItem("flyora_token",data.token);
  localStorage.setItem("flyora_user",JSON.stringify(data));
}
function logout(){localStorage.removeItem("flyora_token");localStorage.removeItem("flyora_user");location.href="/index.html"}
function updateNav(){
  const el=document.getElementById("navAuth");
  if(el && token()){el.textContent="Logout";el.href="#";el.onclick=(e)=>{e.preventDefault();logout()};}
}
function searchFromHome(){
  const o=document.getElementById("homeOrigin").value.trim();
  const d=document.getElementById("homeDestination").value.trim();
  location.href="/flights.html?from="+encodeURIComponent(o)+"&to="+encodeURIComponent(d);
}
document.addEventListener("DOMContentLoaded",updateNav);
