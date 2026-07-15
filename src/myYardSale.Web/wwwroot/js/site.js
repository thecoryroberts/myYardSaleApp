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

    // Auto-dismiss alerts after 5 seconds
    setTimeout(() => {
        document.querySelectorAll('.alert-dismissible').forEach(alert => {
            const bsAlert = bootstrap.Alert.getOrCreateInstance(alert);
            bsAlert.close();
        });
    }, 5000);

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