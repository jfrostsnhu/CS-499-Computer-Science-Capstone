function formatPhoneNumber() {
    const phoneInput = document.querySelector("input[placeholder=' Please enter your phone number.']");
    if (phoneInput) {
        phoneInput.addEventListener("input", function () {
            let digits = this.value.replace(/\D/g, ""); // Remove non-digit characters

            if (!digits) {
                this.value = ""; // Allow empty input
                return;
            }

            // Enforce 10-digit limit
            if (digits.length > 10) {
                digits = digits.substring(0, 10);
            }

            // Format as 123-456-7890
            if (digits.length > 6) {
                this.value = `${digits.substring(0, 3)}-${digits.substring(3, 6)}-${digits.substring(6)}`;
            } else if (digits.length > 3) {
                this.value = `${digits.substring(0, 3)}-${digits.substring(3)}`;
            } else {
                this.value = digits;
            }
        });
    }
}