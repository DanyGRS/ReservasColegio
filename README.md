# Sistema de Reservas - Colégio Vencer Sempre

Este sistema foi desenvolvido em ASP.NET Core para ajudar no controle de empréstimos de equipamentos como projetores, notebooks e caixas de som de Colégios.

---

## Funcionalidades
- Login com autenticação JWT
- Cadastro de usuários, equipamentos, permissões e diretivas
- Realização e visualização de reservas
- Aprovação ou cancelamento de reservas por administradores
- Bloqueio de edição de reservas aprovadas ou que não são do próprio usuário

---

## Tecnologias
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- JWT
- Bootstrap 5 e Bootstrap Icons

---

## Conceitos de POO Utilizados
- Classes e objetos como `Usuario`, `Reserva`, `Equipamento`
- Encapsulamento com validações via anotações
- Polimorfismo com enums e permissões por perfil
- Herança planejada para futuras melhorias

---

## Regras de Permissão
- Apenas administradores podem alterar o status ou usuário da reserva
- Funcionários só podem editar suas próprias reservas pendentes


---

## Observações
Este é um projeto acadêmico em fase final, com foco em aprendizado de autenticação, controle de acesso e estruturação em camadas MVC.


