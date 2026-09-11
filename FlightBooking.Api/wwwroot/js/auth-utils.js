function saveAuth(data) {
    if (!data || !data.token) {
        throw new Error("Authentication token was not returned by the server.");
    }

    localStorage.setItem("flyora_token", data.token);
    localStorage.setItem("flyora_user", JSON.stringify(data));
}

function getAuthToken() {
    return localStorage.getItem("flyora_token");
}

function isLoggedIn() {
    return !!localStorage.getItem("flyora_token");
}

function getCurrentUser() {
    try {
        return JSON.parse(localStorage.getItem("flyora_user") || "null");
    } catch {
        return null;
    }
}

function logout() {
    localStorage.removeItem("flyora_token");
    localStorage.removeItem("flyora_user");

    window.location.href = "/index.html";
}
