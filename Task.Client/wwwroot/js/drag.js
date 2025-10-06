// File: wwwroot/js/drag.js
window.initializeDragBoard = (dotNetHelper) => {
    const columns = document.querySelectorAll('.drag-column .dropzone');
    columns.forEach(column => {
        new Sortable(column, {
            group: 'drag-group', // Renamed group
            animation: 150,
            onEnd: function (evt) {
                const taskId = evt.item.dataset.taskId;
                const newStatus = evt.to.dataset.status;
                dotNetHelper.invokeMethodAsync('OnTaskDropped', taskId, newStatus);
            }
        });
    });
};