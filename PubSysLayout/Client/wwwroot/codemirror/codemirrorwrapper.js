window.codemirror = {
    cm: {},
    changes: {},
    create: function (textArea, initialCode, width, height, mode) {

        this.cm[textArea.id] = CodeMirror.fromTextArea(textArea, {
            mode: mode,
            lineNumbers: true,
            lineWrapping: true,
            matchBrackets: true,
            matchTags: { bothTags: false },
            extraKeys: {
                "Ctrl-Space": "autocomplete",
                "Alt-F": "findPersistent",
                "Ctrl-S": function (cm) { /*only for preventdefault*/ }
            },
            hintOptions: {
                tables: {
                    Articles: ["id_article", "id_metaarticle", "id_server", "id_arttype", "id_user", "title", "annotation", "textfield1", "textfield2", "imageurl", "id_image", "redirurl", "created", "edited", "released", "datereleased", "datestop", "datesort", "chaptercount", "keywords", "options", "discussion", "forum", "forumitems", "forumlastitem", "complete", "del", "tmstamp", "tag", "SEOtitle", "checked", "specfield1", "specfield2", "premium"],
                    Sections: ["id_section", "id_metasection", "id_server", "id_section_parent", "id_section_parent_top", "id_file", "treelevel", "name", "redirurl", "target", "visible", "del", "order", "options", "tag"],
                    ArticleSections: ["id_article", "id_section", "priority", "datesort"],
                    Files: ["id_file", "id_folder", "id_user", "name", "filename", "filesize", "fileext", "image", "width", "height", "upload_date", "tag", "description", "foptions"],
                    ArticleFiles: ["id_file", "id_article", "id_articlefile", "description"],
                    Users: ["id_user", "id_server", "login_name", "lastname", "firstname", "full_name", "password", "description", "user_text", "email", "author", "enabled", "locked", "del", "new", "createdate", "id_file_photo", "id_file_photo_big", "lastPwdChangedDate"],
                    Authors: ["id_article", "id_user", "options"],
                    Chapters: ["id_chapter", "id_article", "id_user", "name", "body", "order", "charcount", "wordcount", "entered", "editdate", "active", "tmstamp", "del", "tag"],
                    ArticleTypes: ["id_arttype", "id_doctype", "description", "id_server", "del", "options"],
                    ArticleTypeOptions: ["id_arttype", "id_option", "oname", "order", "imply"],
                    StatArticlesMonth: ["id_article", "year", "month", "impressions"],
                    StatArticlesDay: ["id_article", "date", "impressions", "impressions_first", "impressions_forum", "impressions_gallery", "impressions_mobile"],
                    Forum: ["id_forum", "id_forum_parent", "id_article", "author_name", "author_email", "title", "body", "date", "mail_notify", "client_IP", "identification", "tmstamp", "collapse", "browser", "last_edit", "edit_count"]
                }
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
    },
    setOption: function (id, option, val) {
        this.cm[id].setOption(option, val);
    }
};
