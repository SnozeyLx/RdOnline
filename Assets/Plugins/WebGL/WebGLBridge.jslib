mergeInto(LibraryManager.library, {
    getReactData: function () {
        if (typeof window.getReactData === "function") {
            let data = window.getReactData(); // Chama a função global do React
            console.log("getReactData chamado pelo Unity WebGL!", data);
            return allocate(intArrayFromString(data), 'i8', ALLOC_NORMAL);
        } else {
            console.warn("getReactData não está definido!");
            return allocate(intArrayFromString("Erro: React não carregado!"), 'i8', ALLOC_NORMAL);
        }
    }
});