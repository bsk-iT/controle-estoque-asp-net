# Documentação de Design - inventarii

Este documento detalha as especificações visuais e estruturais para a replicação do sistema de controle de estoque **inventarii**, focado em uma estética minimalista, elegante e funcional.

---

## 1. Identidade Visual e Marca

### Logotipo
- **Nome:** inventarii (minúsculo, mantendo a sofisticação).
- **Tipografia do Logo:** Serifada clássica (ex: Playfair Display ou similar), com peso regular e espaçamento entre letras (letter-spacing) levemente reduzido para elegância.
- **Símbolo:** Ícone minimalista inspirado em uma casa/armazém com formas abstratas internas (caixas), utilizando linhas finas (stroke-width: 1px ou 1.5px).
- **Escala:** O conjunto ícone + texto deve ser pequeno e delicado, seguindo a proporção da referência original.

---

## 2. Paleta de Cores (Temas)

O sistema foi projetado com suporte nativo a Temas Claro (Padrão) e Escuro.

### Tema Claro (Padrão)
- **Fundo (Background):** #FFFFFF (Branco Puro)
- **Texto Primário:** #000000 (Preto)
- **Bordas e Linhas:** #000000 (ou cinza muito escuro para suavizar, ex: #1A1A1A)
- **Ilustrações:** Estilo "Chalk Art" ou traço fino em preto sobre fundo branco.

### Tema Escuro
- **Fundo (Background):** Aqui testar primeiramente o modo dark do Tailwind para deixar o mais parecido do preto #000000 (Preto Puro) 
- **Texto Primário:** Aqui utilizar o branco mais claro da base Gray do Tailwind #FFFFFF (Branco)
- **Bordas e Linhas:** #FFFFFF
- **Ilustrações:** Estilo "Chalk Art" invertido (branco sobre preto).

---

## 3. Tipografia

- **Títulos (H1, H2):** Serifada (ex: Playfair Display, Lora). Tamanho grande para o Hero, mas com peso refinado.
- **Corpo e Menu:** Sans-serif minimalista (ex: Inter, Roboto, ou Montserrat) ou uma Serifada muito leve para manter a consistência.
- **Idioma:** Português do Brasil (PT-BR).

---

## 4. Componentes da Interface

### Cabeçalho (Header)
- **Navegação:** Links para `Fornecedores`, `Produtos`, `Movimentações`, `Relatórios` e `Contato`.
- **Botão de Alternância de Tema:** Posicionado à esquerda do botão "Entrar". Utiliza ícones de Sol/Lua.
- **Botões de Ação:** 
  - `Entrar`: Botão com borda fina (outlined) e cantos levemente arredondados.
  - `Cadastre-se`: Botão com preenchimento sólido (preto no tema claro, branco no tema escuro).

### Seção Hero
- **Lado Esquerdo:** 
  - Título impactante em Português.
  - Call to Action (CTA): Botão "Crie sua conta" com borda.
  - Link secundário: "Já é um membro? Entre."
- **Lado Direito:** 
  - Ilustração em estilo artístico (armazém, prateleiras ou empilhadeira) que ocupe cerca de 50% da largura da tela.

---

## 5. Estrutura Técnica Sugerida

- **Layout:** Flexbox ou CSS Grid para garantir o alinhamento centralizado e a divisão 50/50 na seção principal.
- **Responsividade:** O menu deve se transformar em um menu hambúrguer em dispositivos móveis, e a ilustração deve ser movida para baixo do texto.
- **Transição de Tema:** Recomenda-se o uso de Variáveis CSS (Custom Properties) para gerenciar as cores de fundo e texto, facilitando a troca via JavaScript ao clicar no botão de tema.

---

## 6. Referências de Imagens
As ilustrações devem manter o estilo de desenho manual ou "blueprint" para reforçar a identidade única do sistema.