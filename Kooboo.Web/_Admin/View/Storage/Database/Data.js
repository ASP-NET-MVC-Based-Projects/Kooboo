$(function() {
  var databaseModel = function() {
    var self = this;

    this.name = ko.observable(Kooboo.getQueryString("table"));

    this.createDataUrl = function() {
      return Kooboo.Route.Get(Kooboo.Route.Database.EditDataPage, {
        table: self.name()
      });
    };

    this.columnsPageUrl = function() {
      return Kooboo.Route.Get(Kooboo.Route.Database.ColumnsPage, {
        table: self.name(),
        from: "Data"
      });
    };

    this.pager = ko.observable();

    Kooboo.EventBus.subscribe("kb/pager/change", function(page) {
      getData(page);
    });

    function getData(page) {
      Kooboo.Database.getData({
        table: self.name(),
        pageNr: page || 1
      }).then(function(res) {
        if (res.success) {
          self.pager(res.model);

          var docs = [],
            columns = [];

          if (res.model.list && res.model.list.length) {
            res.model.list.forEach(function(obj) {
              var model = {};
              self.columns().forEach(function(col) {
                var data = "";
                var find = obj.find(function(item) {
                  return item.key == col.name;
                });
                if (find) {
                  data = find.value.toString();
                }
                model[col.name] = data;
              });
              let id = obj.find(function(item) {
                return item.key == "_id";
              }).value;
              model._origId = id;
              model.edit = {
                text: Kooboo.text.common.edit,
                url: Kooboo.Route.Get(Kooboo.Route.Database.EditDataPage, {
                  id: id,
                  table: self.name()
                })
              };

              docs.push(model);
            });
          }

          self.columns().forEach(function(col) {
            if (!col.isSystem) {
              columns.push({
                displayName: col.name,
                fieldName: col.name,
                type: "text"
              });
            }
          });

          self.tableData({
            docs: docs,
            columns: columns,
            tableActions: [
              {
                fieldName: "edit",
                type: "link-btn"
              }
            ],
            kbType: Kooboo.Database.name,
            class: "table-bordered",
            onDelete: function(list) {
              if (confirm(Kooboo.text.confirm.deleteItems)) {
                var ids = list.map(function(o) {
                  return o._origId;
                });

                Kooboo.Database.deleteData({
                  tableName: self.name(),
                  values: ids
                }).then(function(res) {
                  if (res.success) {
                    getData();
                    window.info.done(Kooboo.text.info.delete.success);
                  }
                });
              }
            }
          });
        }
      });
    }

    this.columns = ko.observableArray();

    Kooboo.Database.getColumns({
      table: self.name()
    }).then(function(res) {
      if (res.success) {
        self.columns(
          res.model.map(function(col) {
            return {
              name: col.name,
              isSystem: col.isSystem
            };
          })
        );

        getData();
      }
    });
  };

  databaseModel.prototype = new Kooboo.tableModel(Kooboo.Database.name);

  var vm = new databaseModel();

  ko.applyBindings(vm, document.getElementById("main"));
});
