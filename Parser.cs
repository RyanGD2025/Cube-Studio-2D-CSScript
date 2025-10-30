using System;
using System.Collections.Generic;

namespace CubeStudioScriptCompiler
{
    public class Parser
    {
        private readonly Lexer _lexer;
        private Token _currentToken;

        public Parser(Lexer lexer)
        {
            _lexer = lexer;
            // Carrega o primeiro token para começar
            _currentToken = _lexer.GetNextToken(); 
        }

        // --- Funções de Ajuda do Parser ---
        private void Consume(TipoToken expectedType)
        {
            if (_currentToken.Tipo == expectedType)
            {
                _currentToken = _lexer.GetNextToken();
            }
            else
            {
                throw new Exception($"ERRO DE SINTAXE: Esperado {expectedType}, mas encontrado {_currentToken.Tipo} ('{_currentToken.Lexema}')");
            }
        }

        private void Expect(TipoToken expectedType)
        {
            if (_currentToken.Tipo != expectedType)
            {
                throw new Exception($"ERRO DE SINTAXE: Esperado {expectedType}, mas encontrado {_currentToken.Tipo} ('{_currentToken.Lexema}')");
            }
        }

        // --- Funções Principais do Parser ---

        // Analisa um valor simples (Número, String, True, False, Identificador)
        private AstNode ParseSimpleExpression()
        {
            if (_currentToken.Tipo == TipoToken.NUMERO || _currentToken.Tipo == TipoToken.STRING ||
                _currentToken.Tipo == TipoToken.TRUE || _currentToken.Tipo == TipoToken.FALSE)
            {
                var node = new LiteralNode(_currentToken.Tipo, _currentToken.Lexema);
                Consume(_currentToken.Tipo);
                return node;
            }
            if (_currentToken.Tipo == TipoToken.IDENTIFICADOR)
            {
                // Por enquanto, trata o identificador como um literal de variável para simplificar
                var node = new LiteralNode(_currentToken.Tipo, _currentToken.Lexema); 
                Consume(_currentToken.Tipo);
                return node;
            }

            throw new Exception($"ERRO DE SINTAXE: Expressao Invalida ('{_currentToken.Lexema}')");
        }

        // Analisa: print.log.Console("Texto");
        private CallStatementNode ParseCallStatement()
        {
            var node = new CallStatementNode();

            // 1. Analisa a cadeia de chamadas (print.log.Console)
            Expect(TipoToken.IDENTIFICADOR);
            node.Path.Add(_currentToken.Lexema);
            Consume(TipoToken.IDENTIFICADOR);

            while (_currentToken.Tipo == TipoToken.PONTO)
            {
                Consume(TipoToken.PONTO);
                Expect(TipoToken.IDENTIFICADOR);
                node.Path.Add(_currentToken.Lexema);
                Consume(TipoToken.IDENTIFICADOR);
            }

            // 2. Analisa os Argumentos (("Texto"))
            if (_currentToken.Tipo == TipoToken.PARENTESES_ABRE)
            {
                Consume(TipoToken.PARENTESES_ABRE);

                if (_currentToken.Tipo != TipoToken.PARENTESES_FECHA)
                {
                    // Assume que há pelo menos 1 argumento
                    node.Arguments.Add(ParseSimpleExpression());
                    
                    // Trata argumentos separados por vírgula
                    while (_currentToken.Tipo == TipoToken.VIRGULA)
                    {
                        Consume(TipoToken.VIRGULA);
                        node.Arguments.Add(ParseSimpleExpression());
                    }
                }
                Consume(TipoToken.PARENTESES_FECHA);
            }
            
            Consume(TipoToken.PONTO_E_VIRGULA); // Toda instrução deve terminar com ;
            return node;
        }

        // Analisa: local nome = valor;
        private LocalDeclarationNode ParseLocalDeclaration()
        {
            Consume(TipoToken.LOCAL); // Consome 'local'
            
            Expect(TipoToken.IDENTIFICADOR);
            var name = _currentToken.Lexema;
            Consume(TipoToken.IDENTIFICADOR);

            AstNode initialValue = null;
            if (_currentToken.Tipo == TipoToken.IGUAL)
            {
                Consume(TipoToken.IGUAL); // Consome '='
                initialValue = ParseSimpleExpression(); 
            }
            
            Consume(TipoToken.PONTO_E_VIRGULA); // Consome ';'
            return new LocalDeclarationNode(name, initialValue);
        }

        // Analisa uma única instrução (chamada, declaração, if, while, etc.)
        public StatementNode ParseStatement()
        {
            if (_currentToken.Tipo == TipoToken.LOCAL)
            {
                return ParseLocalDeclaration();
            }
            // Adicione aqui a lógica para IF, FUNCTION, WHILE...
            
            // Se for um identificador, assumimos que é uma chamada encadeada (print.log.Console)
            if (_currentToken.Tipo == TipoToken.IDENTIFICADOR)
            {
                return ParseCallStatement();
            }

            throw new Exception($"ERRO DE SINTAXE: Instrucao nao reconhecida: {_currentToken.Lexema}");
        }

        // Função principal que analisa todo o código fonte
        public List<StatementNode> ParseProgram()
        {
            var program = new List<StatementNode>();
            while (_currentToken.Tipo != TipoToken.EOF)
            {
                program.Add(ParseStatement());
            }
            return program;
        }
    }
}
