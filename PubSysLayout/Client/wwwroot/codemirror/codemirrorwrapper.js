window.codemirror = {
    cm: {},
    changes: {},
    create: function (textArea, initialCode, width, height, mode) {

        this.cm[textArea.id] = CodeMirror.fromTextArea(textArea, {
            //value: initialCode,
            mode: mode, //"text/css",//"application/x-aspx",
            lineNumbers: true,
            matchBrackets: true,
            matchTags: { bothTags: false },
            //indentUnit: 4,
            //indentWithTabs: true,
            extraKeys: {
                "Alt-F": "findPersistent",
                /*,
                            "F11": function (cm) {
                                cm.setOption("fullScreen", !cm.getOption("fullScreen"));
                            },
                            "Esc": function (cm) {
                                if (cm.getOption("fullScreen")) cm.setOption("fullScreen", false);
                            }*/
                "Ctrl-S": function (cm) { /*alert("save")*/ }
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
