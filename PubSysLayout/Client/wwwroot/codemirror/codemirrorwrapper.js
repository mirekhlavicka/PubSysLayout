window.codemirror = {
    cm: {},
    create: function (textArea, initialCode, width, height) {

        this.cm[textArea.id] = CodeMirror.fromTextArea(textArea, {
            //value: initialCode,
            mode: "application/x-aspx",
            lineNumbers: true,
            matchBrackets: true,
            matchTags: { bothTags: false },
            //indentUnit: 4,
            //indentWithTabs: true,
            extraKeys: {
                "Alt-F": "findPersistent"/*,
                            "F11": function (cm) {
                                cm.setOption("fullScreen", !cm.getOption("fullScreen"));
                            },
                            "Esc": function (cm) {
                                if (cm.getOption("fullScreen")) cm.setOption("fullScreen", false);
                            }*/
            }
        });
        this.cm[textArea.id].setSize(width, height);
        this.cm[textArea.id].setValue(initialCode);
        this.cm[textArea.id].focus();

        //return new Promise(() => { });
    },
    getValue: function (id) {
        return this.cm[id].getValue();
    },
    setValue: function (id, val) {
        return this.cm[id].setValue(val);
    }
};
