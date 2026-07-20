// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Smooth scroll for anchor links
document.addEventListener('DOMContentLoaded', function() {
    // Smooth scroll for same-page links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function(e) {
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                e.preventDefault();
                target.scrollIntoView({ behavior: 'smooth', block: 'start' });
            }
        });
    });

    // Auto-dismiss alerts after 5 seconds using SweetAlert2
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        if (alert.classList.contains('alert-success')) {
            Swal.fire({
                icon: 'success',
                title: alert.textContent.trim() || 'Success',
                timer: 3000,
                showConfirmButton: false,
                toast: true,
                position: 'top-end'
            });
        } else if (alert.classList.contains('alert-danger') || alert.classList.contains('alert-error')) {
            Swal.fire({
                icon: 'error',
                title: alert.textContent.trim() || 'Error',
                timer: 4000,
                showConfirmButton: false,
                toast: true,
                position: 'top-end'
            });
        } else if (alert.classList.contains('alert-warning')) {
            Swal.fire({
                icon: 'warning',
                title: alert.textContent.trim() || 'Warning',
                timer: 3500,
                showConfirmButton: false,
                toast: true,
                position: 'top-end'
            });
        } else if (alert.classList.contains('alert-info')) {
            Swal.fire({
                icon: 'info',
                title: alert.textContent.trim() || 'Info',
                timer: 3000,
                showConfirmButton: false,
                toast: true,
                position: 'top-end'
            });
        }
    });

    // Add loading state to forms
    document.querySelectorAll('form').forEach(form => {
        form.addEventListener('submit', function() {
            const submitBtn = this.querySelector('button[type="submit"]');
            if (submitBtn && !submitBtn.disabled) {
                submitBtn.disabled = true;
                const originalText = submitBtn.innerHTML;
                submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Processing...';
                submitBtn.setAttribute('data-original-text', originalText);
            }
        });
    });

    // Image preview before upload
    const imageInputs = document.querySelectorAll('input[type="file"][accept*="image"]');
    imageInputs.forEach(input => {
        input.addEventListener('change', function() {
            if (this.files && this.files[0]) {
                const reader = new FileReader();
                const previewContainer = this.closest('.mb-3')?.querySelector('.image-preview');
                
                if (previewContainer) {
                    reader.onload = e => {
                        previewContainer.innerHTML = `<img src="${e.target.result}" class="img-thumbnail mt-2" style="max-height: 150px;" />`;
                    };
                    reader.readAsDataURL(this.files[0]);
                }
            }
        });
    });

    // Back to top button (if exists)
    const backToTop = document.querySelector('#backToTop');
    if (backToTop) {
        window.addEventListener('scroll', () => {
            backToTop.style.display = window.scrollY > 300 ? 'block' : 'none';
        });
        backToTop.addEventListener('click', () => {
            window.scrollTo({ top: 0, behavior: 'smooth' });
        });
    }
});

// Utility function to format currency
window.formatCurrency = function(amount) {
    return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD'
    }).format(amount);
};