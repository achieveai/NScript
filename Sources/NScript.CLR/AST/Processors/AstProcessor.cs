//-----------------------------------------------------------------------
// <copyright file="AstProcessor.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST.Processors
{
    using System;
    using System.Collections.Generic;
    using NScript.CLR.AST;

    /// <summary>
    /// Definition for AstProcessor
    /// </summary>
    public class Processor : IAstProcessor
    {
        /// <summary>
        /// Mapping for Type to list of Processors.
        /// </summary>
        private static readonly Dictionary<Type, List<Func<IAstProcessor, Node, Node>>> nodeProcessorMap =
            new Dictionary<Type, List<Func<IAstProcessor, Node, Node>>>();

        private static List<Func<IAstProcessor, List<Statement>, List<Statement>>> statementsProcessor =
            new List<Func<IAstProcessor, List<Statement>, List<Statement>>>();

        /// <summary>
        /// Backing field for context.
        /// </summary>
        private TopLevelBlock context;

        /// <summary>
        /// Initializes the <see cref="Processor"/> class.
        /// </summary>
        static Processor()
        {
            Processor.RegisterProcessor<TypeCheckExpression>(IntCastSimplifier.ProcessTypeCheckExpression);
            Processor.RegisterProcessor<BinaryExpression>(IsTypeProcessor.ProcessBinaryExpression);
            Processor.RegisterProcessor<ConditionalOperatorExpression>(ConditionalOperatorProcessor.Process);
            Processor.RegisterProcessor<UnaryExpression>(NotConditionSimplifier.ProcessNotOperator);
            Processor.RegisterProcessor<BinaryExpression>(NotConditionSimplifier.ProcessEqualToFalseOperator);
            Processor.RegisterProcessor<IfBlockStatement>(IfBlockToReturnConditional.Process);
            Processor.RegisterProcessor<InitObjectWithDefaultValue>(IntiObjectSimplifier.Process);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Processor"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public Processor(TopLevelBlock context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        public TopLevelBlock Context
        {
            get { return this.context; }
        }

        /// <summary>
        /// Processes the specified context.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>Converted Node or the original one.</returns>
        public Node Process(Node node)
        {
            if (node == null)
            {
                return null;
            }

            // First process all the children of this node.
            node.ProcessThroughPipeline(this);

            List<Func<IAstProcessor, Node, Node>> processors;

            bool loop;
            do
            {
                loop = false;

                if (Processor.nodeProcessorMap.TryGetValue(node.GetType(), out processors))
                {
                    for (int index = 0; index < processors.Count; index++)
                    {
                        Node returnValue = processors[index](this, node);

                        if (returnValue != node)
                        {
                            // We need to call Process again on the returnValue to make sure all the processors
                            // have been executed.
                            node = returnValue;

                            loop = true;
                            break;
                        }
                    }
                }
            } while (loop);

            return node;
        }

        /// <summary>
        /// Processes the specified statements.
        /// </summary>
        /// <param name="statements">The statements.</param>
        /// <returns>Processed Statements.</returns>
        public List<Statement> Process(List<Statement> statements)
        {
            if (statements == null)
            {
                return null;
            }

            bool loop;
            do
            {
                loop = false;

                for (int index = 0; index < Processor.statementsProcessor.Count; index++)
                {
                    List<Statement> returnValue = Processor.statementsProcessor[index](this, statements);

                    if (returnValue != statements && returnValue != null)
                    {
                        // We need to call Process again on the returnValue to make sure all the processors
                        // have been executed.
                        statements = returnValue;

                        loop = true;
                        break;
                    }
                }
            } while (loop);

            return statements;
        }

        /// <summary>
        /// Registers the processor.
        /// </summary>
        /// <typeparam name="T">Type deriving from Node</typeparam>
        /// <param name="processor">The processor.</param>
        public static void RegisterProcessor<T>(Func<IAstProcessor, T, Node> processor) where T : Node
        {
            Type type = typeof(T);

            List<Func<IAstProcessor, Node, Node>> nodeProcessors;

            if (!Processor.nodeProcessorMap.TryGetValue(type, out nodeProcessors))
            {
                nodeProcessors = new List<Func<IAstProcessor, Node, Node>>();
                Processor.nodeProcessorMap.Add(type, nodeProcessors);
            }

            nodeProcessors.Add((tlb, n) => processor(tlb, (T)n));
        }
    }
}
