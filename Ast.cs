using System;
using System.Collections.Generic;

namespace CubeStudioScriptCompiler
{
    // A classe base para todos os elementos na Árvore Sintática Abstrata (AST)
    public abstract class AstNode { }

    // Representa um valor literal (Número, String, True, False)
    public class LiteralNode : AstNode
    {
        public TipoToken Tipo { get; } // Ex: NUMERO, STRING
        public string Valor { get; }

        public LiteralNode(TipoToken tipo, string valor)
        {
            Tipo = tipo;
            Valor = valor;
        }
    }

    // Representa uma instrução completa que termina com ponto e vírgula
    public abstract class StatementNode : AstNode { }

    // Representa uma Chamada Encadeada (Ex: print.log.Console("Texto"))
    public class CallStatementNode : StatementNode
    {
        // O caminho completo da chamada (Ex: [print, log, Console])
        public List<string> Path { get; } = new List<string>(); 

        // Os argumentos dentro dos parênteses (Ex: "Texto")
        public List<AstNode> Arguments { get; } = new List<AstNode>(); 
    }

    // Representa uma Declaração de Variável (Ex: local x = 10)
    public class LocalDeclarationNode : StatementNode
    {
        public string Name { get; }
        public AstNode InitialValue { get; } // O valor inicial (10)

        public LocalDeclarationNode(string name, AstNode initialValue)
        {
            Name = name;
            InitialValue = initialValue;
        }
    }
    
    // ... (Outras estruturas como IfNode, WhileNode, FunctionNode serão adicionadas depois)
}
