document.addEventListener("DOMContentLoaded", () => {
    // Functions to open and close a modal
    function openModal($el) {
        if (confirm("Door het opnieuw te koppelen van een badge wordt de oude vergeten. Wilt u doorgaan?") === true) {
            $el.classList.add("is-active");
            ModalTriggered();
        };
    }

    function closeModal($el) {
        $el.classList.remove("is-active");
        location.reload();
    }

    function closeAllModals() {
        (document.querySelectorAll(".modal") || []).forEach(($modal) => {
            closeModal($modal);
        });
    }

    // Add a click event on buttons to open a specific modal
    (document.querySelectorAll(".js-modal-trigger") || []).forEach(($trigger) => {
        const modal = $trigger.dataset.target;
        const $target = document.getElementById(modal);

        $trigger.addEventListener("click", () => {
            openModal($target);
        });
    });

    // Add a click event on various child elements to close the parent modal
    (document.querySelectorAll(
        ".modal-background, .modal-close, .modal-card-head, .delete, .button"
    ) || []
    ).forEach(($close) => {
        const $target = $close.closest(".modal");

        $close.addEventListener("click", () => {
            closeModal($target);
        });
    });

    // Add a keyboard event to close all modals
    document.addEventListener("keydown", (event) => {
        if (event.code === "Escape") {
            closeAllModals();
        }
    });
});

function ModalTriggered() {
    window.$.ajax({
        type: "POST",
        url: "ticket_beheer.aspx/ModalTriggered",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function () {
        },
        error: function () {
        }
    });
};
