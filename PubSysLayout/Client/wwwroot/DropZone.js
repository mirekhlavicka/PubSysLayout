export function initializeFileDropZone(element, inputFile) {
    function pasteHandler(e) {
        inputFile.files = e.clipboardData.files;
        const event = new Event('change', { bubbles: true });
        inputFile.dispatchEvent(event);
    }

    function onDrop(e) {
        e.preventDefault();
        element.classList.remove("hover");

        inputFile.files = e.dataTransfer.files;
        const event = new Event('change', { bubbles: true });
        inputFile.dispatchEvent(event);
    }

    function addDragClass (e) {
        e.preventDefault();
        element.classList.add("hover");
    }

    function removeDragClass (e) {
        e.preventDefault();
        element.classList.remove("hover");
    }

    if (element == null) {
        return {
            dispose: () => { }
        };
    }

    element.addEventListener('paste', pasteHandler);
    element.addEventListener("dragenter", addDragClass);
    element.addEventListener("dragover", addDragClass);
    element.addEventListener("dragleave", removeDragClass);
    element.addEventListener("drop", onDrop);

    return {
        dispose: () => {
            element.removeEventListener('paste', pasteHandler);
            element.removeEventListener('dragenter', addDragClass);
            element.removeEventListener('dragover', addDragClass);
            element.removeEventListener('dragleave', removeDragClass);
            element.removeEventListener("drop", onDrop);
            }
    }
}