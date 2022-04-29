window.codemirror = {
    cm: {},
    changes: {},
    create: function (textArea, initialCode, width, height, mode) {

        this.cm[textArea.id] = CodeMirror.fromTextArea(textArea, {
            mode: mode,
            lineNumbers: true,
            matchBrackets: true,
            matchTags: { bothTags: false },
            extraKeys: {
                "Alt-F": "findPersistent",
                "Ctrl-S": function (cm) { /*only for preventdefault*/ }
            }
        });
        this.cm[textArea.id].setSize(width, height);
        this.cm[textArea.id].setValue(initialCode);

        this.changes[textArea.id] = false;
        this.cm[textArea.id].on('change', editor => {
            this.changes[textArea.id] = true;
        });

        this.cm[textArea.id].focus();

        //return new Promise(() => { });
    },
    getValue: function (id) {
        return this.cm[id].getValue();
    },
    setValue: function (id, val) {
        return this.cm[id].setValue(val);
    },
    getChanges: function (id) {
        return this.changes[id]
    },
    setChanges: function (id, val) {
        return this.changes[id] = val;
    }

};
