Vue.component("kb-page-widget-meta", {
  template: Kooboo.getTemplate(
    "/_Admin/Scripts/pageEditor/widgets/html-meta.html"
  ),
  props: {
    multiLangs: Object
  },
  data: function() {
    return {
      possibleMetaName: [
        "application-name",
        "author",
        "description",
        "generator",
        "keywords",
        "revised",
        "slurp",
        "referrer",
        "creator",
        "publisher",
        "googlebot",
        "robots",
        "viewport"
      ],
      possibleHttpEquiv: [
        "",
        "content-language",
        "content-security-policy",
        "content-type",
        "default-style",
        "refresh",
        "set-cookie"
      ],
      possibleCharset: [
        "US-ASCII",
        "ISO_8859-1:1987",
        "ISO_8859-2:1987",
        "ISO_8859-3:1988",
        "ISO_8859-4:1988",
        "ISO_8859-5:1988",
        "ISO_8859-6:1987",
        "ISO_8859-7:1987",
        "ISO_8859-8:1988",
        "ISO_8859-9:1989",
        "ISO-8859-10",
        "ISO_6937-2-add",
        "JIS_X0201",
        "JIS_Encoding",
        "Shift_JIS",
        "Extended_UNIX_Code_Packed_Format_for_Japanese",
        "Extended_UNIX_Code_Fixed_Width_for_Japanese",
        "BS_4730",
        "SEN_850200_C",
        "IT",
        "ES",
        "DIN_66003",
        "NS_4551-1",
        "NF_Z_62-010",
        "ISO-10646-UTF-1",
        "ISO_646.basic:1983",
        "INVARIANT",
        "ISO_646.irv:1983",
        "NATS-SEFI",
        "NATS-SEFI-ADD",
        "NATS-DANO",
        "NATS-DANO-ADD",
        "SEN_850200_B",
        "KS_C_5601-1987",
        "ISO-2022-KR",
        "EUC-KR",
        "ISO-2022-JP",
        "ISO-2022-JP-2",
        "JIS_C6220-1969-jp",
        "JIS_C6220-1969-ro",
        "PT",
        "greek7-old",
        "latin-greek",
        "NF_Z_62-010_(1973)",
        "Latin-greek-1",
        "ISO_5427",
        "JIS_C6226-1978",
        "BS_viewdata",
        "INIS",
        "INIS-8",
        "INIS-cyrillic",
        "ISO_5427:1981",
        "ISO_5428:1980",
        "GB_1988-80",
        "GB_2312-80",
        "NS_4551-2",
        "videotex-suppl",
        "PT2",
        "ES2",
        "MSZ_7795.3",
        "JIS_C6226-1983",
        "greek7",
        "ASMO_449",
        "iso-ir-90",
        "JIS_C6229-1984-a",
        "JIS_C6229-1984-b",
        "JIS_C6229-1984-b-add",
        "JIS_C6229-1984-hand",
        "JIS_C6229-1984-hand-add",
        "JIS_C6229-1984-kana",
        "ISO_2033-1983",
        "ANSI_X3.110-1983",
        "T.61-7bit",
        "T.61-8bit",
        "ECMA-cyrillic",
        "CSA_Z243.4-1985-1",
        "CSA_Z243.4-1985-2",
        "CSA_Z243.4-1985-gr",
        "ISO_8859-6-E",
        "ISO_8859-6-I",
        "T.101-G2",
        "ISO_8859-8-E",
        "ISO_8859-8-I",
        "CSN_369103",
        "JUS_I.B1.002",
        "IEC_P27-1",
        "JUS_I.B1.003-serb",
        "JUS_I.B1.003-mac",
        "greek-ccitt",
        "NC_NC00-10:81",
        "ISO_6937-2-25",
        "GOST_19768-74",
        "ISO_8859-supp",
        "ISO_10367-box",
        "latin-lap",
        "JIS_X0212-1990",
        "DS_2089",
        "us-dk",
        "dk-us",
        "KSC5636",
        "UNICODE-1-1-UTF-7",
        "ISO-2022-CN",
        "ISO-2022-CN-EXT",
        "UTF-8",
        "ISO-8859-13",
        "ISO-8859-14",
        "ISO-8859-15",
        "ISO-8859-16",
        "GBK",
        "GB18030",
        "OSD_EBCDIC_DF04_15",
        "OSD_EBCDIC_DF03_IRV",
        "OSD_EBCDIC_DF04_1",
        "ISO-11548-1",
        "KZ-1048",
        "ISO-10646-UCS-2",
        "ISO-10646-UCS-4",
        "ISO-10646-UCS-Basic",
        "ISO-10646-Unicode-Latin1",
        "ISO-10646-J-1",
        "ISO-Unicode-IBM-1261",
        "ISO-Unicode-IBM-1268",
        "ISO-Unicode-IBM-1276",
        "ISO-Unicode-IBM-1264",
        "ISO-Unicode-IBM-1265",
        "UNICODE-1-1",
        "SCSU",
        "UTF-7",
        "UTF-16BE",
        "UTF-16LE",
        "UTF-16",
        "CESU-8",
        "UTF-32",
        "UTF-32BE",
        "UTF-32LE",
        "BOCU-1",
        "ISO-8859-1-Windows-3.0-Latin-1",
        "ISO-8859-1-Windows-3.1-Latin-1",
        "ISO-8859-2-Windows-Latin-2",
        "ISO-8859-9-Windows-Latin-5",
        "hp-roman8",
        "Adobe-Standard-Encoding",
        "Ventura-US",
        "Ventura-International",
        "DEC-MCS",
        "IBM850",
        "PC8-Danish-Norwegian",
        "IBM862",
        "PC8-Turkish",
        "IBM-Symbols",
        "IBM-Thai",
        "HP-Legal",
        "HP-Pi-font",
        "HP-Math8",
        "Adobe-Symbol-Encoding",
        "HP-DeskTop",
        "Ventura-Math",
        "Microsoft-Publishing",
        "Windows-31J",
        "GB2312",
        "Big5",
        "macintosh",
        "IBM037",
        "IBM038",
        "IBM273",
        "IBM274",
        "IBM275",
        "IBM277",
        "IBM278",
        "IBM280",
        "IBM281",
        "IBM284",
        "IBM285",
        "IBM290",
        "IBM297",
        "IBM420",
        "IBM423",
        "IBM424",
        "IBM437",
        "IBM500",
        "IBM851",
        "IBM852",
        "IBM855",
        "IBM857",
        "IBM860",
        "IBM861",
        "IBM863",
        "IBM864",
        "IBM865",
        "IBM868",
        "IBM869",
        "IBM870",
        "IBM871",
        "IBM880",
        "IBM891",
        "IBM903",
        "IBM904",
        "IBM905",
        "IBM918",
        "IBM1026",
        "EBCDIC-AT-DE",
        "EBCDIC-AT-DE-A",
        "EBCDIC-CA-FR",
        "EBCDIC-DK-NO",
        "EBCDIC-DK-NO-A",
        "EBCDIC-FI-SE",
        "EBCDIC-FI-SE-A",
        "EBCDIC-FR",
        "EBCDIC-IT",
        "EBCDIC-PT",
        "EBCDIC-ES",
        "EBCDIC-ES-A",
        "EBCDIC-ES-S",
        "EBCDIC-UK",
        "EBCDIC-US",
        "UNKNOWN-8BIT",
        "MNEMONIC",
        "MNEM",
        "VISCII",
        "VIQR",
        "KOI8-R",
        "HZ-GB-2312",
        "IBM866",
        "IBM775",
        "KOI8-U",
        "IBM00858",
        "IBM00924",
        "IBM01140",
        "IBM01141",
        "IBM01142",
        "IBM01143",
        "IBM01144",
        "IBM01145",
        "IBM01146",
        "IBM01147",
        "IBM01148",
        "IBM01149",
        "Big5-HKSCS",
        "IBM1047",
        "PTCP154",
        "Amiga-1251",
        "KOI7-switched",
        "BRF",
        "TSCII",
        "CP51932",
        "windows-874",
        "windows-1250",
        "windows-1251",
        "windows-1252",
        "windows-1253",
        "windows-1254",
        "windows-1255",
        "windows-1256",
        "windows-1257",
        "windows-1258",
        "TIS-620",
        "CP50220"
      ],
      isShow: false,
      name: "",
      httpEquiv: "",
      charset: "",
      multiContents: [],
      enableName: true,
      enableHttpEquiv: true,
      enableCharset: true,
      metaBindingHelpers: [],
      elem: null
    };
  },
  mounted: function() {
    var self = this;
    Kooboo.EventBus.subscribe("kb/page/ComponentStore/change", function() {
      self.metaBindingHelpers = Kooboo.pageEditor.store.ComponentStore.getMetaBindings();
    });

    $("#meta_modal").on("shown.bs.modal", function() {
      $(".autosize")
        .textareaAutoSize()
        .trigger("keyup");
    });

    Kooboo.EventBus.subscribe("kb/html/meta/dialog/show", function(data) {
      self.elem = data.elem;

      var cultures = Object.keys(self.multiLangs.cultures),
        defaultCultureIdx = cultures.indexOf(self.multiLangs.default);

      if (defaultCultureIdx > -1) {
        var defaultCulture = cultures[defaultCultureIdx],
          content = {
            name: defaultCulture,
            value: data.content[""] || data.content[defaultCulture] || "",
            isDefault: true,
            show: true,
            enable: true
          };

        content = Vue.observable(content);
        self.$watch(
          function() {
            return content.show;
          },
          function(show) {
            show &&
              setTimeout(function() {
                $(".autosize")
                  .textareaAutoSize()
                  .trigger("keyup");
              }, 20);
          }
        );

        self.multiContents.push(content);
        cultures.splice(defaultCultureIdx, 1);
      }

      cultures.forEach(function(lang) {
        var content = {
          name: lang,
          value: data.content[lang] || "",
          isDefault: false,
          show: false,
          enable: true
        };

        content = Vue.observable(content);
        self.$watch(
          function() {
            return content.show;
          },
          function(show) {
            show &&
              setTimeout(function() {
                $(".autosize")
                  .textareaAutoSize()
                  .trigger("keyup");
              }, 20);
          }
        );

        self.multiContents.push(content);
      });

      data.name && (self.name = data.name);
      data.httpequiv && (self.httpEquiv = data.httpequiv);
      data.charset && (self.charset = data.charset);
      self.isShow = true;
      Kooboo.EventBus.publish("kb/multilang/get");
    });

    Kooboo.EventBus.subscribe("kb/multilang/change", function(target) {
      var content = _.findLast(self.multiContents, function(content) {
        return target.name == content.name;
      });
      content && (content.show = target.selected);
    });
  },
  methods: {
    reset: function() {
      var self = this;
      self.name = "";
      self.httpEquiv = "";
      self.charset = "";
      self.multiContents = [];
      self.isShow = false;
    },
    metaHelp: function(meta, content) {
      content.value = content.value + "{" + meta + "}";
    },
    save: function() {
      var self = this;
      var meta = {
        name: "",
        httpequiv: "",
        charset: "",
        content: {},
        elem: self.elem
      };

      if (self.name || self.httpEquiv || self.charset) {
        if (self.enableName) {
          meta.name = self.name;
          meta.httpequiv = meta.charset = "";
          _.forEach(self.multiContents, function(content) {
            // if (content.show()) {
            meta.content[content.name] = content.value;
            // }
          });
        } else if (self.enableHttpEquiv) {
          meta.httpequiv = self.httpEquiv;
          meta.name = meta.charset = "";
          _.forEach(self.multiContents, function(content) {
            if (content.show) {
              meta.content[content.name] = content.value;
            }
          });
        } else if (self.enableCharset) {
          meta.charset = self.charset;
          meta.name = meta.httpequiv = "";
          meta.content = {};
        }
        Kooboo.EventBus.publish("kb/page/meta/save", meta);
        self.reset();
      } else {
        alert(Kooboo.text.confirm.invalidMeta);
      }
    },
    enableAll: function() {
      var self = this;
      self.enableName = true;
      self.enableHttpEquiv = true;
      self.enableCharset = true;
      self.enableMultiContents = true;
    }
  },
  watch: {
    name: function(val) {
      var self = this;
      if (val) {
        self.enableName = true;
        self.enableHttpEquiv = false;
        self.enableCharset = false;
        self.enableMultiContents = true;
      } else {
        self.enableAll();
      }
    },
    httpEquiv: function(val) {
      var self = this;
      if (val) {
        self.enableName = false;
        self.enableHttpEquiv = true;
        self.enableCharset = false;
        self.enableMultiContents = true;
      } else {
        self.enableAll();
      }
    },
    charset: function(val) {
      var self = this;
      if (val) {
        self.enableName = false;
        self.enableHttpEquiv = false;
        self.enableCharset = true;
        self.enableMultiContents = false;
      } else {
        self.enableAll();
      }
    }
  }
});
