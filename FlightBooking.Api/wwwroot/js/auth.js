let captchaId = "";

async function loadCaptcha() {
    try {
        const response = await fetch("/api/auth/captcha");

        if (!response.ok) {
            throw new Error("Unable to load security check.");
        }

        const data = await response.json();

        captchaId = data.id;

        document
            .querySelectorAll("#captchaQuestion")
            .forEach(element => {
                element.textContent =
                    "Security check: " + data.question;
            });

    } catch (error) {
        console.error("CAPTCHA error:", error);

        document
            .querySelectorAll("#captchaQuestion")
            .forEach(element => {
                element.textContent =
                    "Security check unavailable";
            });
    }
}


function getErrorMessage(data, fallback = "Something went wrong.") {

    // Plain string response
    if (typeof data === "string") {
        return data;
    }

    // ASP.NET ProblemDetails
    if (data?.detail) {
        return data.detail;
    }

    // Custom message
    if (data?.message) {
        return data.message;
    }

    // Validation errors
    if (data?.errors) {
        const messages = [];

        Object.values(data.errors).forEach(value => {
            if (Array.isArray(value)) {
                messages.push(...value);
            } else if (typeof value === "string") {
                messages.push(value);
            }
        });

        if (messages.length > 0) {
            return messages.join(" ");
        }
    }

    if (data?.title) {
        return data.title;
    }

    return fallback;
}


function msg(text, success = false) {

    const element = document.getElementById("message");

    if (!element) {
        return;
    }

    element.textContent = text;

    element.className =
        "message" + (success ? " success" : "");
}


async function readResponse(response) {

    const body = await response.text();

    if (!body) {
        return null;
    }

    try {
        return JSON.parse(body);
    } catch {
        return body;
    }
}

function saveAuth(data) {
    if (!data || !data.token) {
        throw new Error("Login response did not contain an authentication token.");
    }

    // Use the same storage keys used by app.js
    localStorage.setItem("flyora_token", data.token);
    localStorage.setItem("flyora_user", JSON.stringify(data));
}

document.addEventListener("DOMContentLoaded", async () => {

    await loadCaptcha();


    // =========================
    // LOGIN
    // =========================

    const loginForm =
        document.getElementById("loginForm");

    if (loginForm) {

        loginForm.onsubmit = async event => {

            event.preventDefault();

            try {

                const response = await fetch(
                    "/api/auth/login",
                    {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json"
                        },
                        body: JSON.stringify({
                            identifier:
                                document.getElementById("identifier").value,

                            password:
                                document.getElementById("password").value,

                            captchaId:
                                captchaId,

                            captchaAnswer:
                                document.getElementById("captchaAnswer").value
                        })
                    }
                );

                const data =
                    await readResponse(response);

                if (!response.ok) {

                    throw new Error(
                        getErrorMessage(
                            data,
                            "Login failed."
                        )
                    );
                }

                saveAuth(data);

                const next =
                    new URLSearchParams(location.search)
                        .get("next") ||
                    "/flights.html";

                location.href = next;

            } catch (error) {

                console.error(error);

                msg(
                    error.message ||
                    "Login failed."
                );

                await loadCaptcha();
            }
        };
    }


    // =========================
    // SIGN UP
    // =========================

    const signupForm =
        document.getElementById("signupForm");

    if (signupForm) {

        signupForm.onsubmit = async event => {

            event.preventDefault();

            try {

                const response = await fetch(
                    "/api/auth/register",
                    {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json"
                        },
                        body: JSON.stringify({

                            username:
                                document.getElementById("username").value,

                            firstName:
                                document.getElementById("firstName").value,

                            lastName:
                                document.getElementById("lastName").value,

                            dob:
                                document.getElementById("dob").value,

                            phone:
                                document.getElementById("phone").value,

                            email:
                                document.getElementById("email").value,

                            password:
                                document.getElementById("password").value,

                            captchaId:
                                captchaId,

                            captchaAnswer:
                                document.getElementById("captchaAnswer").value
                        })
                    }
                );

                const data =
                    await readResponse(response);

                if (!response.ok) {

                    throw new Error(
                        getErrorMessage(
                            data,
                            "Registration failed."
                        )
                    );
                }

                saveAuth(data);

                location.href =
                    "/flights.html";

            } catch (error) {

                console.error(error);

                msg(
                    error.message ||
                    "Registration failed."
                );

                await loadCaptcha();
            }
        };
    }


    // =========================
    // FORGOT PASSWORD
    // =========================

    const forgotForm =
        document.getElementById("forgotForm");

    if (forgotForm) {

        forgotForm.onsubmit = async event => {

            event.preventDefault();

            try {

                const response = await fetch(
                    "/api/auth/forgot-password",
                    {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json"
                        },
                        body: JSON.stringify({

                            identifier:
                                document.getElementById("identifier").value,

                            phone:
                                document.getElementById("phone").value,

                            newPassword:
                                document.getElementById("password").value,

                            captchaId:
                                captchaId,

                            captchaAnswer:
                                document.getElementById("captchaAnswer").value
                        })
                    }
                );

                const data =
                    await readResponse(response);

                if (!response.ok) {

                    throw new Error(
                        getErrorMessage(
                            data,
                            "Password reset failed."
                        )
                    );
                }

                msg(
                    getErrorMessage(
                        data,
                        "Password reset successfully."
                    ),
                    true
                );

                setTimeout(
                    () => {
                        location.href =
                            "/login.html";
                    },
                    1000
                );

            } catch (error) {

                console.error(error);

                msg(
                    error.message ||
                    "Password reset failed."
                );

                await loadCaptcha();
            }
        };
    }
});
