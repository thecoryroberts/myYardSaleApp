// ===== myYardSale Modern JavaScript =====

// ===== Dark Mode Module =====
const DarkMode = {
  init() {
    const storedTheme = localStorage.getItem('theme');
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
    const isDark = storedTheme ? storedTheme === 'dark' : prefersDark;

    if (isDark) {
      document.body.setAttribute('data-theme', 'dark');
    }

    // Listen for system theme changes
    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', e => {
      if (!localStorage.getItem('theme')) {
        document.body.setAttribute('data-theme', e.matches ? 'dark' : 'light');
      }
    });

    // Bind toggle events
    this.bindToggles();
  },

  toggle(button) {
    const isDark = document.body.getAttribute('data-theme') === 'dark';
    const newTheme = isDark ? 'light' : 'dark';

    document.body.setAttribute('data-theme', newTheme);
    localStorage.setItem('theme', newTheme);

    // Update icon
    const icon = button.querySelector('i');
    if (icon) {
      icon.className = isDark ? 'bi bi-moon' : 'bi bi-sun';
    }
  },

  bindToggles() {
    document.querySelectorAll('.dark-mode-toggle').forEach(btn => {
      btn.addEventListener('click', () => this.toggle(btn));
    });
  }
};

// ===== Navigation Module =====
const Navigation = {
  init() {
    this.initStickyHeader();
    this.initSmoothScroll();
    this.initBackToTop();
  },

  initStickyHeader() {
    const header = document.querySelector('.site-header');
    if (!header) return;

    const handleScroll = () => {
      const scrolled = window.scrollY > 10;
      header.classList.toggle('scrolled', scrolled);
    };

    window.addEventListener('scroll', handleScroll, { passive: true });
    handleScroll();
  },

  initSmoothScroll() {
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
      anchor.addEventListener('click', function(e) {
        const target = document.querySelector(this.getAttribute('href'));
        if (target) {
          e.preventDefault();
          target.scrollIntoView({ behavior: 'smooth', block: 'start' });
        }
      });
    });
  },

  initBackToTop() {
    const backToTop = document.getElementById('backToTop');
    if (!backToTop) return;

    const handleScroll = () => {
      backToTop.classList.toggle('show', window.scrollY > 300);
    };

    window.addEventListener('scroll', handleScroll, { passive: true });
    backToTop.addEventListener('click', () => {
      window.scrollTo({ top: 0, behavior: 'smooth' });
    });

    handleScroll();
  }
};

// ===== Forms Module =====
const Forms = {
  init() {
    this.initFilePreviews();
    this.initLoadingStates();
    this.initPriceFormatting();
  },

  initFilePreviews() {
    const imageInputs = document.querySelectorAll('input[type="file"][accept*="image"]');
    imageInputs.forEach(input => {
      input.addEventListener('change', function() {
        if (!this.files || !this.files[0]) return;

        const previewContainer = this.closest('.mb-3')?.querySelector('.image-preview');
        if (!previewContainer) return;

        const reader = new FileReader();
        reader.onload = e => {
          previewContainer.innerHTML = `<img src="${e.target.result}" class="img-thumbnail mt-2" style="max-height: 150px;" loading="lazy" />`;
        };
        reader.readAsDataURL(this.files[0]);
      });
    });
  },

  initLoadingStates() {
    document.querySelectorAll('form').forEach(form => {
      form.addEventListener('submit', function() {
        const submitBtn = this.querySelector('button[type="submit"]');
        if (submitBtn && !submitBtn.disabled) {
          submitBtn.disabled = true;
          const originalText = submitBtn.innerHTML;
          submitBtn.setAttribute('data-original-text', originalText);
          submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Processing...';
        }
      });
    });
  },

  initPriceFormatting() {
    const priceInput = document.querySelector('input[name="Price"]');
    if (!priceInput) return;

    priceInput.addEventListener('blur', function() {
      if (this.value) {
        const val = parseFloat(this.value);
        if (!isNaN(val)) {
          this.value = val.toFixed(2);
        }
      }
    });
  }
};

// ===== Alerts & Notifications Module =====
const Alerts = {
  init() {
    this.initSweetAlerts();
  },

  initSweetAlerts() {
    if (typeof Swal === 'undefined') return;

    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
      if (alert.classList.contains('alert-success')) {
        const toast = Swal.fire({
          icon: 'success',
          title: alert.textContent.trim() || 'Success',
          timer: 3000,
          showConfirmButton: false,
          toast: true,
          position: 'top-end'
        });

        alert.style.display = 'none';
      } else if (alert.classList.contains('alert-danger') || alert.classList.contains('alert-error')) {
        Swal.fire({
          icon: 'error',
          title: alert.textContent.trim() || 'Error',
          timer: 4000,
          showConfirmButton: false,
          toast: true,
          position: 'top-end'
        });
        alert.style.display = 'none';
      } else if (alert.classList.contains('alert-warning')) {
        Swal.fire({
          icon: 'warning',
          title: alert.textContent.trim() || 'Warning',
          timer: 3500,
          showConfirmButton: false,
          toast: true,
          position: 'top-end'
        });
        alert.style.display = 'none';
      } else if (alert.classList.contains('alert-info')) {
        Swal.fire({
          icon: 'info',
          title: alert.textContent.trim() || 'Info',
          timer: 3000,
          showConfirmButton: false,
          toast: true,
          position: 'top-end'
        });
        alert.style.display = 'none';
      }
    });
  }
};

// ===== Accessibility Module =====
const Accessibility = {
  init() {
    this.initLazyImageLoading();
    this.initSkipLink();
  },

  initLazyImageLoading() {
    const images = document.querySelectorAll('img[loading="lazy"]');
    images.forEach(img => {
      img.addEventListener('load', function() {
        this.classList.add('loaded');
      });
      // If image is already cached and loaded
      if (img.complete) {
        img.classList.add('loaded');
      }
    });
  },

  initSkipLink() {
    const skipLink = document.querySelector('.skip-link');
    if (!skipLink) return;

    const handleKeyDown = (e) => {
      if (e.key === 'Tab' && !e.shiftKey) {
        const mainContent = document.querySelector('#mainContent');
        if (mainContent && document.activeElement === skipLink) {
          // Skip link is being tabbed away from, ensure focus moves to main content
          mainContent.setAttribute('tabindex', '-1');
          mainContent.focus();
        }
      }
    };

    document.addEventListener('keydown', handleKeyDown);
  }
};

// ===== Image Handling Module =====
const Images = {
  init() {
    this.initLightbox();
  },

  initLightbox() {
    const modalEl = document.getElementById('lightboxModal');
    if (!modalEl) return;

    const img = document.getElementById('lightboxImage');
    if (!img) return;

    const bsModal = bootstrap.Modal.getOrCreateInstance(modalEl);

    document.querySelectorAll('.listing-thumb').forEach(el => {
      el.addEventListener('click', function() {
        img.src = this.dataset.full || this.src;
        bsModal.show();
      });
    });

    modalEl.addEventListener('hidden.bs.modal', function() {
      img.src = '';
    });
  }
};

// ===== Animations Module =====
const Animations = {
  init() {
    this.initStaggerAnimations();
    this.initScrollAnimations();
  },

  initStaggerAnimations() {
    document.querySelectorAll('.listing-card').forEach((card, index) => {
      card.style.animationDelay = `${index * 50}ms`;
    });
  },

  initScrollAnimations() {
    const observer = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          entry.target.classList.add('animate-fade-in-up');
          observer.unobserve(entry.target);
        }
      });
    }, {
      threshold: 0.1,
      rootMargin: '0px 0px -50px 0px'
    });

    document.querySelectorAll('.animate-on-scroll').forEach(el => {
      observer.observe(el);
    });
  }
};

// ===== Main Application =====
document.addEventListener('DOMContentLoaded', function() {
  // Initialize all modules
  DarkMode.init();
  Navigation.init();
  Forms.init();
  Alerts.init();
  Accessibility.init();
  Images.init();
  Animations.init();

  // ===== Live Search Filters =====
  const filterForm = document.querySelector('.filter-bar');
  if (filterForm) {
    const searchInput = filterForm.querySelector('input[name="searchTerm"]');
    const categorySelect = filterForm.querySelector('select[name="categoryId"]');
    const sortSelect = filterForm.querySelector('select[name="sortBy"]');

    // Live filter on change for selects
    [categorySelect, sortSelect].forEach(el => {
      if (!el) return;
      el.addEventListener('change', function() {
        filterForm.submit();
      });
    });

    // Debounced search
    let searchTimer;
    if (searchInput) {
      searchInput.addEventListener('input', function() {
        clearTimeout(searchTimer);
        searchTimer = setTimeout(function() {
          filterForm.submit();
        }, 350);
      });
    }
  }

  console.log('myYardSale UI initialized successfully');
});

// ===== Utility Functions =====
window.formatCurrency = function(amount) {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD'
  }).format(amount);
};

// Export modules for testing if needed
if (typeof module !== 'undefined' && module.exports) {
  module.exports = {
    DarkMode,
    Navigation,
    Forms,
    Alerts,
    Accessibility,
    Images,
    Animations
  };
}