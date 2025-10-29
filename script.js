// Cube Studio 2D CSScript - Simulador simples

const editor = document.getElementById('editor');
const output = document.getElementById('output');
const runBtn = document.getElementById('runBtn');

// Lista de keywords e funções para destaque
const keywords = ["local","function","if","else","return","elseif"];
const functions = ["Add","Destroy","Move","Rotate","Scale","RandomMove","RandomRotate","RandomScale","WaitSprite","WaitFirstChild","GetService","GetChildren"];

runBtn.addEventListener('click', () => {
    const code = editor.value;
    output.textContent = ''; // limpa saída

    const lines = code.split('\n');
    lines.forEach((line, i) => {
        line = line.trim();
        if(!line) return; // ignora linhas vazias

        // Simula comentário
        if(line.startsWith('//')) {
            output.textContent += `[Comentário] ${line}\n`;
            return;
        }

        // Destaca keywords
        let lineOut = line;
        keywords.forEach(kw => {
            const regex = new RegExp(`\\b${kw}\\b`, 'g');
            lineOut = lineOut.replace(regex, `[Keyword:${kw}]`);
        });

        // Destaca funções com {}
        functions.forEach(fn => {
            const regex = new RegExp(`\\b${fn}\\{\\}`, 'g');
            lineOut = lineOut.replace(regex, `[Função:${fn}]`);
        });

        // Destaca strings
        lineOut = lineOut.replace(/"(.*?)"/g, '[String:$1]');

        // Destaca propriedades
        const props = [".Name",".Parent",".Image",".Color",".Pos",".Size",".Anchored"];
        props.forEach(p => {
            const regex = new RegExp(`\\${p}`, 'g');
            lineOut = lineOut.replace(regex, `[Propriedade:${p}]`);
        });

        // Destaca números, true, false
        lineOut = lineOut.replace(/\b([0-9]+|true|false)\b/g, '[Número:$1]');

        output.textContent += lineOut + '\n';
    });
});
