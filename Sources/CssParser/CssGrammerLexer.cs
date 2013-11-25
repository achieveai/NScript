// $ANTLR 3.3.0.7239 CssGrammer.g3 2013-11-23 21:54:06

// The variable 'variable' is assigned but its value is never used.
#pragma warning disable 219
// Unreachable code detected.
#pragma warning disable 162


using System.Collections.Generic;
using Antlr.Runtime;
using Stack = System.Collections.Generic.Stack<object>;
using List = System.Collections.IList;
using ArrayList = System.Collections.Generic.List<object>;

namespace CssParser
{
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "3.3.0.7239")]
[System.CLSCompliant(false)]
public partial class CssGrammerLexer : Antlr.Runtime.Lexer
{
	public const int EOF=-1;
	public const int ATTRIB=4;
	public const int ATTRIBEQUAL=5;
	public const int Alphabets=6;
	public const int BEGINSWITH=7;
	public const int CLASS=8;
	public const int COLOR=9;
	public const int Comment=10;
	public const int DecimalDigits=11;
	public const int FUNCTION=12;
	public const int HASVALUE=13;
	public const int HexDigits=14;
	public const int ID=15;
	public const int IDENT=16;
	public const int IMPORT=17;
	public const int NEST=18;
	public const int NESTED=19;
	public const int NUM=20;
	public const int NUMARG=21;
	public const int NewLine=22;
	public const int PARENTOF=23;
	public const int PRECEDEDS=24;
	public const int PROPERTY=25;
	public const int PSEUDO=26;
	public const int RULE=27;
	public const int STRING=28;
	public const int SingleLineComment=29;
	public const int SkipSpace=30;
	public const int TAG=31;
	public const int WhiteSpace=32;
	public const int T__33=33;
	public const int T__34=34;
	public const int T__35=35;
	public const int T__36=36;
	public const int T__37=37;
	public const int T__38=38;
	public const int T__39=39;
	public const int T__40=40;
	public const int T__41=41;
	public const int T__42=42;
	public const int T__43=43;
	public const int T__44=44;
	public const int T__45=45;
	public const int T__46=46;
	public const int T__47=47;
	public const int T__48=48;
	public const int T__49=49;
	public const int T__50=50;
	public const int T__51=51;
	public const int T__52=52;
	public const int T__53=53;
	public const int T__54=54;
	public const int T__55=55;
	public const int T__56=56;
	public const int T__57=57;
	public const int T__58=58;
	public const int T__59=59;
	public const int T__60=60;
	public const int T__61=61;
	public const int T__62=62;
	public const int T__63=63;
	public const int T__64=64;
	public const int T__65=65;
	public const int T__66=66;
	public const int T__67=67;
	public const int T__68=68;

	protected bool enumIsKeyword = true;
	protected bool assertIsKeyword = true;


    // delegates
    // delegators

	public CssGrammerLexer()
	{
		OnCreated();
	}

	public CssGrammerLexer(ICharStream input )
		: this(input, new RecognizerSharedState())
	{
	}

	public CssGrammerLexer(ICharStream input, RecognizerSharedState state)
		: base(input, state)
	{


		OnCreated();
	}
	public override string GrammarFileName { get { return "CssGrammer.g3"; } }

	private static readonly bool[] decisionCanBacktrack = new bool[0];


	partial void OnCreated();
	partial void EnterRule(string ruleName, int ruleIndex);
	partial void LeaveRule(string ruleName, int ruleIndex);

	partial void Enter_T__33();
	partial void Leave_T__33();

	// $ANTLR start "T__33"
	[GrammarRule("T__33")]
	private void mT__33()
	{
		Enter_T__33();
		EnterRule("T__33", 1);
		TraceIn("T__33", 1);
		try
		{
			int _type = T__33;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:14:7: ( '#' )
			DebugEnterAlt(1);
			// CssGrammer.g3:14:9: '#'
			{
			DebugLocation(14, 9);
			Match('#'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__33", 1);
			LeaveRule("T__33", 1);
			Leave_T__33();
		}
	}
	// $ANTLR end "T__33"

	partial void Enter_T__34();
	partial void Leave_T__34();

	// $ANTLR start "T__34"
	[GrammarRule("T__34")]
	private void mT__34()
	{
		Enter_T__34();
		EnterRule("T__34", 2);
		TraceIn("T__34", 2);
		try
		{
			int _type = T__34;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:15:7: ( '%' )
			DebugEnterAlt(1);
			// CssGrammer.g3:15:9: '%'
			{
			DebugLocation(15, 9);
			Match('%'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__34", 2);
			LeaveRule("T__34", 2);
			Leave_T__34();
		}
	}
	// $ANTLR end "T__34"

	partial void Enter_T__35();
	partial void Leave_T__35();

	// $ANTLR start "T__35"
	[GrammarRule("T__35")]
	private void mT__35()
	{
		Enter_T__35();
		EnterRule("T__35", 3);
		TraceIn("T__35", 3);
		try
		{
			int _type = T__35;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:16:7: ( '(' )
			DebugEnterAlt(1);
			// CssGrammer.g3:16:9: '('
			{
			DebugLocation(16, 9);
			Match('('); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__35", 3);
			LeaveRule("T__35", 3);
			Leave_T__35();
		}
	}
	// $ANTLR end "T__35"

	partial void Enter_T__36();
	partial void Leave_T__36();

	// $ANTLR start "T__36"
	[GrammarRule("T__36")]
	private void mT__36()
	{
		Enter_T__36();
		EnterRule("T__36", 4);
		TraceIn("T__36", 4);
		try
		{
			int _type = T__36;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:17:7: ( ')' )
			DebugEnterAlt(1);
			// CssGrammer.g3:17:9: ')'
			{
			DebugLocation(17, 9);
			Match(')'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__36", 4);
			LeaveRule("T__36", 4);
			Leave_T__36();
		}
	}
	// $ANTLR end "T__36"

	partial void Enter_T__37();
	partial void Leave_T__37();

	// $ANTLR start "T__37"
	[GrammarRule("T__37")]
	private void mT__37()
	{
		Enter_T__37();
		EnterRule("T__37", 5);
		TraceIn("T__37", 5);
		try
		{
			int _type = T__37;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:18:7: ( '+' )
			DebugEnterAlt(1);
			// CssGrammer.g3:18:9: '+'
			{
			DebugLocation(18, 9);
			Match('+'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__37", 5);
			LeaveRule("T__37", 5);
			Leave_T__37();
		}
	}
	// $ANTLR end "T__37"

	partial void Enter_T__38();
	partial void Leave_T__38();

	// $ANTLR start "T__38"
	[GrammarRule("T__38")]
	private void mT__38()
	{
		Enter_T__38();
		EnterRule("T__38", 6);
		TraceIn("T__38", 6);
		try
		{
			int _type = T__38;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:19:7: ( ',' )
			DebugEnterAlt(1);
			// CssGrammer.g3:19:9: ','
			{
			DebugLocation(19, 9);
			Match(','); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__38", 6);
			LeaveRule("T__38", 6);
			Leave_T__38();
		}
	}
	// $ANTLR end "T__38"

	partial void Enter_T__39();
	partial void Leave_T__39();

	// $ANTLR start "T__39"
	[GrammarRule("T__39")]
	private void mT__39()
	{
		Enter_T__39();
		EnterRule("T__39", 7);
		TraceIn("T__39", 7);
		try
		{
			int _type = T__39;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:20:7: ( '.' )
			DebugEnterAlt(1);
			// CssGrammer.g3:20:9: '.'
			{
			DebugLocation(20, 9);
			Match('.'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__39", 7);
			LeaveRule("T__39", 7);
			Leave_T__39();
		}
	}
	// $ANTLR end "T__39"

	partial void Enter_T__40();
	partial void Leave_T__40();

	// $ANTLR start "T__40"
	[GrammarRule("T__40")]
	private void mT__40()
	{
		Enter_T__40();
		EnterRule("T__40", 8);
		TraceIn("T__40", 8);
		try
		{
			int _type = T__40;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:21:7: ( ':' )
			DebugEnterAlt(1);
			// CssGrammer.g3:21:9: ':'
			{
			DebugLocation(21, 9);
			Match(':'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__40", 8);
			LeaveRule("T__40", 8);
			Leave_T__40();
		}
	}
	// $ANTLR end "T__40"

	partial void Enter_T__41();
	partial void Leave_T__41();

	// $ANTLR start "T__41"
	[GrammarRule("T__41")]
	private void mT__41()
	{
		Enter_T__41();
		EnterRule("T__41", 9);
		TraceIn("T__41", 9);
		try
		{
			int _type = T__41;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:22:7: ( '::' )
			DebugEnterAlt(1);
			// CssGrammer.g3:22:9: '::'
			{
			DebugLocation(22, 9);
			Match("::"); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__41", 9);
			LeaveRule("T__41", 9);
			Leave_T__41();
		}
	}
	// $ANTLR end "T__41"

	partial void Enter_T__42();
	partial void Leave_T__42();

	// $ANTLR start "T__42"
	[GrammarRule("T__42")]
	private void mT__42()
	{
		Enter_T__42();
		EnterRule("T__42", 10);
		TraceIn("T__42", 10);
		try
		{
			int _type = T__42;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:23:7: ( ';' )
			DebugEnterAlt(1);
			// CssGrammer.g3:23:9: ';'
			{
			DebugLocation(23, 9);
			Match(';'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__42", 10);
			LeaveRule("T__42", 10);
			Leave_T__42();
		}
	}
	// $ANTLR end "T__42"

	partial void Enter_T__43();
	partial void Leave_T__43();

	// $ANTLR start "T__43"
	[GrammarRule("T__43")]
	private void mT__43()
	{
		Enter_T__43();
		EnterRule("T__43", 11);
		TraceIn("T__43", 11);
		try
		{
			int _type = T__43;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:24:7: ( '=' )
			DebugEnterAlt(1);
			// CssGrammer.g3:24:9: '='
			{
			DebugLocation(24, 9);
			Match('='); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__43", 11);
			LeaveRule("T__43", 11);
			Leave_T__43();
		}
	}
	// $ANTLR end "T__43"

	partial void Enter_T__44();
	partial void Leave_T__44();

	// $ANTLR start "T__44"
	[GrammarRule("T__44")]
	private void mT__44()
	{
		Enter_T__44();
		EnterRule("T__44", 12);
		TraceIn("T__44", 12);
		try
		{
			int _type = T__44;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:25:7: ( '>' )
			DebugEnterAlt(1);
			// CssGrammer.g3:25:9: '>'
			{
			DebugLocation(25, 9);
			Match('>'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__44", 12);
			LeaveRule("T__44", 12);
			Leave_T__44();
		}
	}
	// $ANTLR end "T__44"

	partial void Enter_T__45();
	partial void Leave_T__45();

	// $ANTLR start "T__45"
	[GrammarRule("T__45")]
	private void mT__45()
	{
		Enter_T__45();
		EnterRule("T__45", 13);
		TraceIn("T__45", 13);
		try
		{
			int _type = T__45;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:26:7: ( '@' )
			DebugEnterAlt(1);
			// CssGrammer.g3:26:9: '@'
			{
			DebugLocation(26, 9);
			Match('@'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__45", 13);
			LeaveRule("T__45", 13);
			Leave_T__45();
		}
	}
	// $ANTLR end "T__45"

	partial void Enter_T__46();
	partial void Leave_T__46();

	// $ANTLR start "T__46"
	[GrammarRule("T__46")]
	private void mT__46()
	{
		Enter_T__46();
		EnterRule("T__46", 14);
		TraceIn("T__46", 14);
		try
		{
			int _type = T__46;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:27:7: ( '@import' )
			DebugEnterAlt(1);
			// CssGrammer.g3:27:9: '@import'
			{
			DebugLocation(27, 9);
			Match("@import"); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__46", 14);
			LeaveRule("T__46", 14);
			Leave_T__46();
		}
	}
	// $ANTLR end "T__46"

	partial void Enter_T__47();
	partial void Leave_T__47();

	// $ANTLR start "T__47"
	[GrammarRule("T__47")]
	private void mT__47()
	{
		Enter_T__47();
		EnterRule("T__47", 15);
		TraceIn("T__47", 15);
		try
		{
			int _type = T__47;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:28:7: ( '@include' )
			DebugEnterAlt(1);
			// CssGrammer.g3:28:9: '@include'
			{
			DebugLocation(28, 9);
			Match("@include"); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__47", 15);
			LeaveRule("T__47", 15);
			Leave_T__47();
		}
	}
	// $ANTLR end "T__47"

	partial void Enter_T__48();
	partial void Leave_T__48();

	// $ANTLR start "T__48"
	[GrammarRule("T__48")]
	private void mT__48()
	{
		Enter_T__48();
		EnterRule("T__48", 16);
		TraceIn("T__48", 16);
		try
		{
			int _type = T__48;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:29:7: ( '[' )
			DebugEnterAlt(1);
			// CssGrammer.g3:29:9: '['
			{
			DebugLocation(29, 9);
			Match('['); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__48", 16);
			LeaveRule("T__48", 16);
			Leave_T__48();
		}
	}
	// $ANTLR end "T__48"

	partial void Enter_T__49();
	partial void Leave_T__49();

	// $ANTLR start "T__49"
	[GrammarRule("T__49")]
	private void mT__49()
	{
		Enter_T__49();
		EnterRule("T__49", 17);
		TraceIn("T__49", 17);
		try
		{
			int _type = T__49;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:30:7: ( ']' )
			DebugEnterAlt(1);
			// CssGrammer.g3:30:9: ']'
			{
			DebugLocation(30, 9);
			Match(']'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__49", 17);
			LeaveRule("T__49", 17);
			Leave_T__49();
		}
	}
	// $ANTLR end "T__49"

	partial void Enter_T__50();
	partial void Leave_T__50();

	// $ANTLR start "T__50"
	[GrammarRule("T__50")]
	private void mT__50()
	{
		Enter_T__50();
		EnterRule("T__50", 18);
		TraceIn("T__50", 18);
		try
		{
			int _type = T__50;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:31:7: ( 'cm' )
			DebugEnterAlt(1);
			// CssGrammer.g3:31:9: 'cm'
			{
			DebugLocation(31, 9);
			Match("cm"); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__50", 18);
			LeaveRule("T__50", 18);
			Leave_T__50();
		}
	}
	// $ANTLR end "T__50"

	partial void Enter_T__51();
	partial void Leave_T__51();

	// $ANTLR start "T__51"
	[GrammarRule("T__51")]
	private void mT__51()
	{
		Enter_T__51();
		EnterRule("T__51", 19);
		TraceIn("T__51", 19);
		try
		{
			int _type = T__51;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:32:7: ( 'deg' )
			DebugEnterAlt(1);
			// CssGrammer.g3:32:9: 'deg'
			{
			DebugLocation(32, 9);
			Match("deg"); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__51", 19);
			LeaveRule("T__51", 19);
			Leave_T__51();
		}
	}
	// $ANTLR end "T__51"

	partial void Enter_T__52();
	partial void Leave_T__52();

	// $ANTLR start "T__52"
	[GrammarRule("T__52")]
	private void mT__52()
	{
		Enter_T__52();
		EnterRule("T__52", 20);
		TraceIn("T__52", 20);
		try
		{
			int _type = T__52;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:33:7: ( 'em' )
			DebugEnterAlt(1);
			// CssGrammer.g3:33:9: 'em'
			{
			DebugLocation(33, 9);
			Match("em"); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__52", 20);
			LeaveRule("T__52", 20);
			Leave_T__52();
		}
	}
	// $ANTLR end "T__52"

	partial void Enter_T__53();
	partial void Leave_T__53();

	// $ANTLR start "T__53"
	[GrammarRule("T__53")]
	private void mT__53()
	{
		Enter_T__53();
		EnterRule("T__53", 21);
		TraceIn("T__53", 21);
		try
		{
			int _type = T__53;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:34:7: ( 'ex' )
			DebugEnterAlt(1);
			// CssGrammer.g3:34:9: 'ex'
			{
			DebugLocation(34, 9);
			Match("ex"); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__53", 21);
			LeaveRule("T__53", 21);
			Leave_T__53();
		}
	}
	// $ANTLR end "T__53"

	partial void Enter_T__54();
	partial void Leave_T__54();

	// $ANTLR start "T__54"
	[GrammarRule("T__54")]
	private void mT__54()
	{
		Enter_T__54();
		EnterRule("T__54", 22);
		TraceIn("T__54", 22);
		try
		{
			int _type = T__54;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:35:7: ( 'grad' )
			DebugEnterAlt(1);
			// CssGrammer.g3:35:9: 'grad'
			{
			DebugLocation(35, 9);
			Match("grad"); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__54", 22);
			LeaveRule("T__54", 22);
			Leave_T__54();
		}
	}
	// $ANTLR end "T__54"

	partial void Enter_T__55();
	partial void Leave_T__55();

	// $ANTLR start "T__55"
	[GrammarRule("T__55")]
	private void mT__55()
	{
		Enter_T__55();
		EnterRule("T__55", 23);
		TraceIn("T__55", 23);
		try
		{
			int _type = T__55;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:36:7: ( 'hz' )
			DebugEnterAlt(1);
			// CssGrammer.g3:36:9: 'hz'
			{
			DebugLocation(36, 9);
			Match("hz"); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__55", 23);
			LeaveRule("T__55", 23);
			Leave_T__55();
		}
	}
	// $ANTLR end "T__55"

	partial void Enter_T__56();
	partial void Leave_T__56();

	// $ANTLR start "T__56"
	[GrammarRule("T__56")]
	private void mT__56()
	{
		Enter_T__56();
		EnterRule("T__56", 24);
		TraceIn("T__56", 24);
		try
		{
			int _type = T__56;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:37:7: ( 'in' )
			DebugEnterAlt(1);
			// CssGrammer.g3:37:9: 'in'
			{
			DebugLocation(37, 9);
			Match("in"); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__56", 24);
			LeaveRule("T__56", 24);
			Leave_T__56();
		}
	}
	// $ANTLR end "T__56"

	partial void Enter_T__57();
	partial void Leave_T__57();

	// $ANTLR start "T__57"
	[GrammarRule("T__57")]
	private void mT__57()
	{
		Enter_T__57();
		EnterRule("T__57", 25);
		TraceIn("T__57", 25);
		try
		{
			int _type = T__57;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:38:7: ( 'khz' )
			DebugEnterAlt(1);
			// CssGrammer.g3:38:9: 'khz'
			{
			DebugLocation(38, 9);
			Match("khz"); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__57", 25);
			LeaveRule("T__57", 25);
			Leave_T__57();
		}
	}
	// $ANTLR end "T__57"

	partial void Enter_T__58();
	partial void Leave_T__58();

	// $ANTLR start "T__58"
	[GrammarRule("T__58")]
	private void mT__58()
	{
		Enter_T__58();
		EnterRule("T__58", 26);
		TraceIn("T__58", 26);
		try
		{
			int _type = T__58;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:39:7: ( 'mm' )
			DebugEnterAlt(1);
			// CssGrammer.g3:39:9: 'mm'
			{
			DebugLocation(39, 9);
			Match("mm"); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__58", 26);
			LeaveRule("T__58", 26);
			Leave_T__58();
		}
	}
	// $ANTLR end "T__58"

	partial void Enter_T__59();
	partial void Leave_T__59();

	// $ANTLR start "T__59"
	[GrammarRule("T__59")]
	private void mT__59()
	{
		Enter_T__59();
		EnterRule("T__59", 27);
		TraceIn("T__59", 27);
		try
		{
			int _type = T__59;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:40:7: ( 'ms' )
			DebugEnterAlt(1);
			// CssGrammer.g3:40:9: 'ms'
			{
			DebugLocation(40, 9);
			Match("ms"); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__59", 27);
			LeaveRule("T__59", 27);
			Leave_T__59();
		}
	}
	// $ANTLR end "T__59"

	partial void Enter_T__60();
	partial void Leave_T__60();

	// $ANTLR start "T__60"
	[GrammarRule("T__60")]
	private void mT__60()
	{
		Enter_T__60();
		EnterRule("T__60", 28);
		TraceIn("T__60", 28);
		try
		{
			int _type = T__60;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:41:7: ( 'pc' )
			DebugEnterAlt(1);
			// CssGrammer.g3:41:9: 'pc'
			{
			DebugLocation(41, 9);
			Match("pc"); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__60", 28);
			LeaveRule("T__60", 28);
			Leave_T__60();
		}
	}
	// $ANTLR end "T__60"

	partial void Enter_T__61();
	partial void Leave_T__61();

	// $ANTLR start "T__61"
	[GrammarRule("T__61")]
	private void mT__61()
	{
		Enter_T__61();
		EnterRule("T__61", 29);
		TraceIn("T__61", 29);
		try
		{
			int _type = T__61;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:42:7: ( 'pt' )
			DebugEnterAlt(1);
			// CssGrammer.g3:42:9: 'pt'
			{
			DebugLocation(42, 9);
			Match("pt"); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__61", 29);
			LeaveRule("T__61", 29);
			Leave_T__61();
		}
	}
	// $ANTLR end "T__61"

	partial void Enter_T__62();
	partial void Leave_T__62();

	// $ANTLR start "T__62"
	[GrammarRule("T__62")]
	private void mT__62()
	{
		Enter_T__62();
		EnterRule("T__62", 30);
		TraceIn("T__62", 30);
		try
		{
			int _type = T__62;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:43:7: ( 'px' )
			DebugEnterAlt(1);
			// CssGrammer.g3:43:9: 'px'
			{
			DebugLocation(43, 9);
			Match("px"); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__62", 30);
			LeaveRule("T__62", 30);
			Leave_T__62();
		}
	}
	// $ANTLR end "T__62"

	partial void Enter_T__63();
	partial void Leave_T__63();

	// $ANTLR start "T__63"
	[GrammarRule("T__63")]
	private void mT__63()
	{
		Enter_T__63();
		EnterRule("T__63", 31);
		TraceIn("T__63", 31);
		try
		{
			int _type = T__63;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:44:7: ( 'rad' )
			DebugEnterAlt(1);
			// CssGrammer.g3:44:9: 'rad'
			{
			DebugLocation(44, 9);
			Match("rad"); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__63", 31);
			LeaveRule("T__63", 31);
			Leave_T__63();
		}
	}
	// $ANTLR end "T__63"

	partial void Enter_T__64();
	partial void Leave_T__64();

	// $ANTLR start "T__64"
	[GrammarRule("T__64")]
	private void mT__64()
	{
		Enter_T__64();
		EnterRule("T__64", 32);
		TraceIn("T__64", 32);
		try
		{
			int _type = T__64;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:45:7: ( 's' )
			DebugEnterAlt(1);
			// CssGrammer.g3:45:9: 's'
			{
			DebugLocation(45, 9);
			Match('s'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__64", 32);
			LeaveRule("T__64", 32);
			Leave_T__64();
		}
	}
	// $ANTLR end "T__64"

	partial void Enter_T__65();
	partial void Leave_T__65();

	// $ANTLR start "T__65"
	[GrammarRule("T__65")]
	private void mT__65()
	{
		Enter_T__65();
		EnterRule("T__65", 33);
		TraceIn("T__65", 33);
		try
		{
			int _type = T__65;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:46:7: ( '{' )
			DebugEnterAlt(1);
			// CssGrammer.g3:46:9: '{'
			{
			DebugLocation(46, 9);
			Match('{'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__65", 33);
			LeaveRule("T__65", 33);
			Leave_T__65();
		}
	}
	// $ANTLR end "T__65"

	partial void Enter_T__66();
	partial void Leave_T__66();

	// $ANTLR start "T__66"
	[GrammarRule("T__66")]
	private void mT__66()
	{
		Enter_T__66();
		EnterRule("T__66", 34);
		TraceIn("T__66", 34);
		try
		{
			int _type = T__66;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:47:7: ( '|=' )
			DebugEnterAlt(1);
			// CssGrammer.g3:47:9: '|='
			{
			DebugLocation(47, 9);
			Match("|="); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__66", 34);
			LeaveRule("T__66", 34);
			Leave_T__66();
		}
	}
	// $ANTLR end "T__66"

	partial void Enter_T__67();
	partial void Leave_T__67();

	// $ANTLR start "T__67"
	[GrammarRule("T__67")]
	private void mT__67()
	{
		Enter_T__67();
		EnterRule("T__67", 35);
		TraceIn("T__67", 35);
		try
		{
			int _type = T__67;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:48:7: ( '}' )
			DebugEnterAlt(1);
			// CssGrammer.g3:48:9: '}'
			{
			DebugLocation(48, 9);
			Match('}'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__67", 35);
			LeaveRule("T__67", 35);
			Leave_T__67();
		}
	}
	// $ANTLR end "T__67"

	partial void Enter_T__68();
	partial void Leave_T__68();

	// $ANTLR start "T__68"
	[GrammarRule("T__68")]
	private void mT__68()
	{
		Enter_T__68();
		EnterRule("T__68", 36);
		TraceIn("T__68", 36);
		try
		{
			int _type = T__68;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:49:7: ( '~=' )
			DebugEnterAlt(1);
			// CssGrammer.g3:49:9: '~='
			{
			DebugLocation(49, 9);
			Match("~="); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("T__68", 36);
			LeaveRule("T__68", 36);
			Leave_T__68();
		}
	}
	// $ANTLR end "T__68"

	partial void Enter_IDENT();
	partial void Leave_IDENT();

	// $ANTLR start "IDENT"
	[GrammarRule("IDENT")]
	private void mIDENT()
	{
		Enter_IDENT();
		EnterRule("IDENT", 37);
		TraceIn("IDENT", 37);
		try
		{
			int _type = IDENT;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:133:2: ( ( '_' | Alphabets | '\\u0100' .. '\\ufffe' ) ( '_' | '-' | Alphabets | '\\u0100' .. '\\ufffe' | DecimalDigits )* | '-' ( '_' | Alphabets | '\\u0100' .. '\\ufffe' ) ( '_' | '-' | Alphabets | '\\u0100' .. '\\ufffe' | DecimalDigits )* )
			int alt3=2;
			try { DebugEnterDecision(3, decisionCanBacktrack[3]);
			int LA3_0 = input.LA(1);

			if (((LA3_0>='A' && LA3_0<='Z')||LA3_0=='_'||(LA3_0>='a' && LA3_0<='z')||(LA3_0>='\u0100' && LA3_0<='\uFFFE')))
			{
				alt3=1;
			}
			else if ((LA3_0=='-'))
			{
				alt3=2;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 3, 0, input);

				DebugRecognitionException(nvae);
				throw nvae;
			}
			} finally { DebugExitDecision(3); }
			switch (alt3)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:133:4: ( '_' | Alphabets | '\\u0100' .. '\\ufffe' ) ( '_' | '-' | Alphabets | '\\u0100' .. '\\ufffe' | DecimalDigits )*
				{
				DebugLocation(133, 4);
				input.Consume();

				DebugLocation(134, 3);
				// CssGrammer.g3:134:3: ( '_' | '-' | Alphabets | '\\u0100' .. '\\ufffe' | DecimalDigits )*
				try { DebugEnterSubRule(1);
				while (true)
				{
					int alt1=2;
					try { DebugEnterDecision(1, decisionCanBacktrack[1]);
					int LA1_0 = input.LA(1);

					if ((LA1_0=='-'||(LA1_0>='0' && LA1_0<='9')||(LA1_0>='A' && LA1_0<='Z')||LA1_0=='_'||(LA1_0>='a' && LA1_0<='z')||(LA1_0>='\u0100' && LA1_0<='\uFFFE')))
					{
						alt1=1;
					}


					} finally { DebugExitDecision(1); }
					switch ( alt1 )
					{
					case 1:
						DebugEnterAlt(1);
						// CssGrammer.g3:
						{
						DebugLocation(134, 3);
						input.Consume();


						}
						break;

					default:
						goto loop1;
					}
				}

				loop1:
					;

				} finally { DebugExitSubRule(1); }


				}
				break;
			case 2:
				DebugEnterAlt(2);
				// CssGrammer.g3:135:4: '-' ( '_' | Alphabets | '\\u0100' .. '\\ufffe' ) ( '_' | '-' | Alphabets | '\\u0100' .. '\\ufffe' | DecimalDigits )*
				{
				DebugLocation(135, 4);
				Match('-'); 
				DebugLocation(135, 8);
				input.Consume();

				DebugLocation(136, 3);
				// CssGrammer.g3:136:3: ( '_' | '-' | Alphabets | '\\u0100' .. '\\ufffe' | DecimalDigits )*
				try { DebugEnterSubRule(2);
				while (true)
				{
					int alt2=2;
					try { DebugEnterDecision(2, decisionCanBacktrack[2]);
					int LA2_0 = input.LA(1);

					if ((LA2_0=='-'||(LA2_0>='0' && LA2_0<='9')||(LA2_0>='A' && LA2_0<='Z')||LA2_0=='_'||(LA2_0>='a' && LA2_0<='z')||(LA2_0>='\u0100' && LA2_0<='\uFFFE')))
					{
						alt2=1;
					}


					} finally { DebugExitDecision(2); }
					switch ( alt2 )
					{
					case 1:
						DebugEnterAlt(1);
						// CssGrammer.g3:
						{
						DebugLocation(136, 3);
						input.Consume();


						}
						break;

					default:
						goto loop2;
					}
				}

				loop2:
					;

				} finally { DebugExitSubRule(2); }


				}
				break;

			}
			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("IDENT", 37);
			LeaveRule("IDENT", 37);
			Leave_IDENT();
		}
	}
	// $ANTLR end "IDENT"

	partial void Enter_STRING();
	partial void Leave_STRING();

	// $ANTLR start "STRING"
	[GrammarRule("STRING")]
	private void mSTRING()
	{
		Enter_STRING();
		EnterRule("STRING", 38);
		TraceIn("STRING", 38);
		try
		{
			int _type = STRING;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:141:2: ( '\"' (~ ( '\"' | '\\n' | '\\r' ) )* '\"' | '\\'' (~ ( '\\'' | '\\n' | '\\r' ) )* '\\'' )
			int alt6=2;
			try { DebugEnterDecision(6, decisionCanBacktrack[6]);
			int LA6_0 = input.LA(1);

			if ((LA6_0=='\"'))
			{
				alt6=1;
			}
			else if ((LA6_0=='\''))
			{
				alt6=2;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 6, 0, input);

				DebugRecognitionException(nvae);
				throw nvae;
			}
			} finally { DebugExitDecision(6); }
			switch (alt6)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:141:4: '\"' (~ ( '\"' | '\\n' | '\\r' ) )* '\"'
				{
				DebugLocation(141, 4);
				Match('\"'); 
				DebugLocation(141, 8);
				// CssGrammer.g3:141:8: (~ ( '\"' | '\\n' | '\\r' ) )*
				try { DebugEnterSubRule(4);
				while (true)
				{
					int alt4=2;
					try { DebugEnterDecision(4, decisionCanBacktrack[4]);
					int LA4_0 = input.LA(1);

					if (((LA4_0>='\u0000' && LA4_0<='\t')||(LA4_0>='\u000B' && LA4_0<='\f')||(LA4_0>='\u000E' && LA4_0<='!')||(LA4_0>='#' && LA4_0<='\uFFFF')))
					{
						alt4=1;
					}


					} finally { DebugExitDecision(4); }
					switch ( alt4 )
					{
					case 1:
						DebugEnterAlt(1);
						// CssGrammer.g3:
						{
						DebugLocation(141, 8);
						input.Consume();


						}
						break;

					default:
						goto loop4;
					}
				}

				loop4:
					;

				} finally { DebugExitSubRule(4); }

				DebugLocation(141, 28);
				Match('\"'); 

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// CssGrammer.g3:142:4: '\\'' (~ ( '\\'' | '\\n' | '\\r' ) )* '\\''
				{
				DebugLocation(142, 4);
				Match('\''); 
				DebugLocation(142, 9);
				// CssGrammer.g3:142:9: (~ ( '\\'' | '\\n' | '\\r' ) )*
				try { DebugEnterSubRule(5);
				while (true)
				{
					int alt5=2;
					try { DebugEnterDecision(5, decisionCanBacktrack[5]);
					int LA5_0 = input.LA(1);

					if (((LA5_0>='\u0000' && LA5_0<='\t')||(LA5_0>='\u000B' && LA5_0<='\f')||(LA5_0>='\u000E' && LA5_0<='&')||(LA5_0>='(' && LA5_0<='\uFFFF')))
					{
						alt5=1;
					}


					} finally { DebugExitDecision(5); }
					switch ( alt5 )
					{
					case 1:
						DebugEnterAlt(1);
						// CssGrammer.g3:
						{
						DebugLocation(142, 9);
						input.Consume();


						}
						break;

					default:
						goto loop5;
					}
				}

				loop5:
					;

				} finally { DebugExitSubRule(5); }

				DebugLocation(142, 30);
				Match('\''); 

				}
				break;

			}
			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("STRING", 38);
			LeaveRule("STRING", 38);
			Leave_STRING();
		}
	}
	// $ANTLR end "STRING"

	partial void Enter_NUM();
	partial void Leave_NUM();

	// $ANTLR start "NUM"
	[GrammarRule("NUM")]
	private void mNUM()
	{
		Enter_NUM();
		EnterRule("NUM", 39);
		TraceIn("NUM", 39);
		try
		{
			int _type = NUM;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:146:2: ( '-' ( ( DecimalDigits )* '.' )? ( DecimalDigits )+ | ( ( DecimalDigits )* '.' )? ( DecimalDigits )+ )
			int alt13=2;
			try { DebugEnterDecision(13, decisionCanBacktrack[13]);
			int LA13_0 = input.LA(1);

			if ((LA13_0=='-'))
			{
				alt13=1;
			}
			else if ((LA13_0=='.'||(LA13_0>='0' && LA13_0<='9')))
			{
				alt13=2;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 13, 0, input);

				DebugRecognitionException(nvae);
				throw nvae;
			}
			} finally { DebugExitDecision(13); }
			switch (alt13)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:146:4: '-' ( ( DecimalDigits )* '.' )? ( DecimalDigits )+
				{
				DebugLocation(146, 4);
				Match('-'); 
				DebugLocation(146, 8);
				// CssGrammer.g3:146:8: ( ( DecimalDigits )* '.' )?
				int alt8=2;
				try { DebugEnterSubRule(8);
				try { DebugEnterDecision(8, decisionCanBacktrack[8]);
				try
				{
					alt8 = dfa8.Predict(input);
				}
				catch (NoViableAltException nvae)
				{
					DebugRecognitionException(nvae);
					throw;
				}
				} finally { DebugExitDecision(8); }
				switch (alt8)
				{
				case 1:
					DebugEnterAlt(1);
					// CssGrammer.g3:146:9: ( DecimalDigits )* '.'
					{
					DebugLocation(146, 9);
					// CssGrammer.g3:146:9: ( DecimalDigits )*
					try { DebugEnterSubRule(7);
					while (true)
					{
						int alt7=2;
						try { DebugEnterDecision(7, decisionCanBacktrack[7]);
						int LA7_0 = input.LA(1);

						if (((LA7_0>='0' && LA7_0<='9')))
						{
							alt7=1;
						}


						} finally { DebugExitDecision(7); }
						switch ( alt7 )
						{
						case 1:
							DebugEnterAlt(1);
							// CssGrammer.g3:
							{
							DebugLocation(146, 9);
							input.Consume();


							}
							break;

						default:
							goto loop7;
						}
					}

					loop7:
						;

					} finally { DebugExitSubRule(7); }

					DebugLocation(146, 24);
					Match('.'); 

					}
					break;

				}
				} finally { DebugExitSubRule(8); }

				DebugLocation(146, 30);
				// CssGrammer.g3:146:30: ( DecimalDigits )+
				int cnt9=0;
				try { DebugEnterSubRule(9);
				while (true)
				{
					int alt9=2;
					try { DebugEnterDecision(9, decisionCanBacktrack[9]);
					int LA9_0 = input.LA(1);

					if (((LA9_0>='0' && LA9_0<='9')))
					{
						alt9=1;
					}


					} finally { DebugExitDecision(9); }
					switch (alt9)
					{
					case 1:
						DebugEnterAlt(1);
						// CssGrammer.g3:
						{
						DebugLocation(146, 30);
						input.Consume();


						}
						break;

					default:
						if (cnt9 >= 1)
							goto loop9;

						EarlyExitException eee9 = new EarlyExitException( 9, input );
						DebugRecognitionException(eee9);
						throw eee9;
					}
					cnt9++;
				}
				loop9:
					;

				} finally { DebugExitSubRule(9); }


				}
				break;
			case 2:
				DebugEnterAlt(2);
				// CssGrammer.g3:147:4: ( ( DecimalDigits )* '.' )? ( DecimalDigits )+
				{
				DebugLocation(147, 4);
				// CssGrammer.g3:147:4: ( ( DecimalDigits )* '.' )?
				int alt11=2;
				try { DebugEnterSubRule(11);
				try { DebugEnterDecision(11, decisionCanBacktrack[11]);
				try
				{
					alt11 = dfa11.Predict(input);
				}
				catch (NoViableAltException nvae)
				{
					DebugRecognitionException(nvae);
					throw;
				}
				} finally { DebugExitDecision(11); }
				switch (alt11)
				{
				case 1:
					DebugEnterAlt(1);
					// CssGrammer.g3:147:5: ( DecimalDigits )* '.'
					{
					DebugLocation(147, 5);
					// CssGrammer.g3:147:5: ( DecimalDigits )*
					try { DebugEnterSubRule(10);
					while (true)
					{
						int alt10=2;
						try { DebugEnterDecision(10, decisionCanBacktrack[10]);
						int LA10_0 = input.LA(1);

						if (((LA10_0>='0' && LA10_0<='9')))
						{
							alt10=1;
						}


						} finally { DebugExitDecision(10); }
						switch ( alt10 )
						{
						case 1:
							DebugEnterAlt(1);
							// CssGrammer.g3:
							{
							DebugLocation(147, 5);
							input.Consume();


							}
							break;

						default:
							goto loop10;
						}
					}

					loop10:
						;

					} finally { DebugExitSubRule(10); }

					DebugLocation(147, 20);
					Match('.'); 

					}
					break;

				}
				} finally { DebugExitSubRule(11); }

				DebugLocation(147, 26);
				// CssGrammer.g3:147:26: ( DecimalDigits )+
				int cnt12=0;
				try { DebugEnterSubRule(12);
				while (true)
				{
					int alt12=2;
					try { DebugEnterDecision(12, decisionCanBacktrack[12]);
					int LA12_0 = input.LA(1);

					if (((LA12_0>='0' && LA12_0<='9')))
					{
						alt12=1;
					}


					} finally { DebugExitDecision(12); }
					switch (alt12)
					{
					case 1:
						DebugEnterAlt(1);
						// CssGrammer.g3:
						{
						DebugLocation(147, 26);
						input.Consume();


						}
						break;

					default:
						if (cnt12 >= 1)
							goto loop12;

						EarlyExitException eee12 = new EarlyExitException( 12, input );
						DebugRecognitionException(eee12);
						throw eee12;
					}
					cnt12++;
				}
				loop12:
					;

				} finally { DebugExitSubRule(12); }


				}
				break;

			}
			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("NUM", 39);
			LeaveRule("NUM", 39);
			Leave_NUM();
		}
	}
	// $ANTLR end "NUM"

	partial void Enter_COLOR();
	partial void Leave_COLOR();

	// $ANTLR start "COLOR"
	[GrammarRule("COLOR")]
	private void mCOLOR()
	{
		Enter_COLOR();
		EnterRule("COLOR", 40);
		TraceIn("COLOR", 40);
		try
		{
			int _type = COLOR;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:151:2: ( '#' ( HexDigits )+ )
			DebugEnterAlt(1);
			// CssGrammer.g3:151:4: '#' ( HexDigits )+
			{
			DebugLocation(151, 4);
			Match('#'); 
			DebugLocation(151, 8);
			// CssGrammer.g3:151:8: ( HexDigits )+
			int cnt14=0;
			try { DebugEnterSubRule(14);
			while (true)
			{
				int alt14=2;
				try { DebugEnterDecision(14, decisionCanBacktrack[14]);
				int LA14_0 = input.LA(1);

				if (((LA14_0>='0' && LA14_0<='9')||(LA14_0>='a' && LA14_0<='f')))
				{
					alt14=1;
				}


				} finally { DebugExitDecision(14); }
				switch (alt14)
				{
				case 1:
					DebugEnterAlt(1);
					// CssGrammer.g3:
					{
					DebugLocation(151, 8);
					input.Consume();


					}
					break;

				default:
					if (cnt14 >= 1)
						goto loop14;

					EarlyExitException eee14 = new EarlyExitException( 14, input );
					DebugRecognitionException(eee14);
					throw eee14;
				}
				cnt14++;
			}
			loop14:
				;

			} finally { DebugExitSubRule(14); }


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("COLOR", 40);
			LeaveRule("COLOR", 40);
			Leave_COLOR();
		}
	}
	// $ANTLR end "COLOR"

	partial void Enter_SingleLineComment();
	partial void Leave_SingleLineComment();

	// $ANTLR start "SingleLineComment"
	[GrammarRule("SingleLineComment")]
	private void mSingleLineComment()
	{
		Enter_SingleLineComment();
		EnterRule("SingleLineComment", 41);
		TraceIn("SingleLineComment", 41);
		try
		{
			int _type = SingleLineComment;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:156:2: ( '//' (~ ( '\\n' | '\\r' ) )* ( '\\n' | '\\r' ( '\\n' )? ) )
			DebugEnterAlt(1);
			// CssGrammer.g3:156:4: '//' (~ ( '\\n' | '\\r' ) )* ( '\\n' | '\\r' ( '\\n' )? )
			{
			DebugLocation(156, 4);
			Match("//"); 

			DebugLocation(157, 3);
			// CssGrammer.g3:157:3: (~ ( '\\n' | '\\r' ) )*
			try { DebugEnterSubRule(15);
			while (true)
			{
				int alt15=2;
				try { DebugEnterDecision(15, decisionCanBacktrack[15]);
				int LA15_0 = input.LA(1);

				if (((LA15_0>='\u0000' && LA15_0<='\t')||(LA15_0>='\u000B' && LA15_0<='\f')||(LA15_0>='\u000E' && LA15_0<='\uFFFF')))
				{
					alt15=1;
				}


				} finally { DebugExitDecision(15); }
				switch ( alt15 )
				{
				case 1:
					DebugEnterAlt(1);
					// CssGrammer.g3:
					{
					DebugLocation(157, 3);
					input.Consume();


					}
					break;

				default:
					goto loop15;
				}
			}

			loop15:
				;

			} finally { DebugExitSubRule(15); }

			DebugLocation(157, 19);
			// CssGrammer.g3:157:19: ( '\\n' | '\\r' ( '\\n' )? )
			int alt17=2;
			try { DebugEnterSubRule(17);
			try { DebugEnterDecision(17, decisionCanBacktrack[17]);
			int LA17_0 = input.LA(1);

			if ((LA17_0=='\n'))
			{
				alt17=1;
			}
			else if ((LA17_0=='\r'))
			{
				alt17=2;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 17, 0, input);

				DebugRecognitionException(nvae);
				throw nvae;
			}
			} finally { DebugExitDecision(17); }
			switch (alt17)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:157:20: '\\n'
				{
				DebugLocation(157, 20);
				Match('\n'); 

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// CssGrammer.g3:157:25: '\\r' ( '\\n' )?
				{
				DebugLocation(157, 25);
				Match('\r'); 
				DebugLocation(157, 29);
				// CssGrammer.g3:157:29: ( '\\n' )?
				int alt16=2;
				try { DebugEnterSubRule(16);
				try { DebugEnterDecision(16, decisionCanBacktrack[16]);
				int LA16_0 = input.LA(1);

				if ((LA16_0=='\n'))
				{
					alt16=1;
				}
				} finally { DebugExitDecision(16); }
				switch (alt16)
				{
				case 1:
					DebugEnterAlt(1);
					// CssGrammer.g3:157:30: '\\n'
					{
					DebugLocation(157, 30);
					Match('\n'); 

					}
					break;

				}
				} finally { DebugExitSubRule(16); }


				}
				break;

			}
			} finally { DebugExitSubRule(17); }

			DebugLocation(158, 3);
			 _channel = Hidden; 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("SingleLineComment", 41);
			LeaveRule("SingleLineComment", 41);
			Leave_SingleLineComment();
		}
	}
	// $ANTLR end "SingleLineComment"

	partial void Enter_Comment();
	partial void Leave_Comment();

	// $ANTLR start "Comment"
	[GrammarRule("Comment")]
	private void mComment()
	{
		Enter_Comment();
		EnterRule("Comment", 42);
		TraceIn("Comment", 42);
		try
		{
			int _type = Comment;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:163:2: ( '/*' ( . )* '*/' )
			DebugEnterAlt(1);
			// CssGrammer.g3:163:4: '/*' ( . )* '*/'
			{
			DebugLocation(163, 4);
			Match("/*"); 

			DebugLocation(163, 9);
			// CssGrammer.g3:163:9: ( . )*
			try { DebugEnterSubRule(18);
			while (true)
			{
				int alt18=2;
				try { DebugEnterDecision(18, decisionCanBacktrack[18]);
				int LA18_0 = input.LA(1);

				if ((LA18_0=='*'))
				{
					int LA18_1 = input.LA(2);

					if ((LA18_1=='/'))
					{
						alt18=2;
					}
					else if (((LA18_1>='\u0000' && LA18_1<='.')||(LA18_1>='0' && LA18_1<='\uFFFF')))
					{
						alt18=1;
					}


				}
				else if (((LA18_0>='\u0000' && LA18_0<=')')||(LA18_0>='+' && LA18_0<='\uFFFF')))
				{
					alt18=1;
				}


				} finally { DebugExitDecision(18); }
				switch ( alt18 )
				{
				case 1:
					DebugEnterAlt(1);
					// CssGrammer.g3:163:9: .
					{
					DebugLocation(163, 9);
					MatchAny(); 

					}
					break;

				default:
					goto loop18;
				}
			}

			loop18:
				;

			} finally { DebugExitSubRule(18); }

			DebugLocation(163, 12);
			Match("*/"); 

			DebugLocation(164, 3);
			 _channel = Hidden; 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("Comment", 42);
			LeaveRule("Comment", 42);
			Leave_Comment();
		}
	}
	// $ANTLR end "Comment"

	partial void Enter_SkipSpace();
	partial void Leave_SkipSpace();

	// $ANTLR start "SkipSpace"
	[GrammarRule("SkipSpace")]
	private void mSkipSpace()
	{
		Enter_SkipSpace();
		EnterRule("SkipSpace", 43);
		TraceIn("SkipSpace", 43);
		try
		{
			int _type = SkipSpace;
			int _channel = DefaultTokenChannel;
			// CssGrammer.g3:168:2: ( NewLine | WhiteSpace )
			int alt19=2;
			try { DebugEnterDecision(19, decisionCanBacktrack[19]);
			int LA19_0 = input.LA(1);

			if ((LA19_0=='\n'||LA19_0=='\r'||(LA19_0>='\u2028' && LA19_0<='\u2029')))
			{
				alt19=1;
			}
			else if ((LA19_0=='\t'||LA19_0=='\f'||LA19_0==' '||LA19_0=='v'||LA19_0=='\u00A0'))
			{
				alt19=2;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 19, 0, input);

				DebugRecognitionException(nvae);
				throw nvae;
			}
			} finally { DebugExitDecision(19); }
			switch (alt19)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:168:4: NewLine
				{
				DebugLocation(168, 4);
				mNewLine(); 
				DebugLocation(169, 3);
				 _channel = Hidden; 

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// CssGrammer.g3:170:4: WhiteSpace
				{
				DebugLocation(170, 4);
				mWhiteSpace(); 
				DebugLocation(171, 3);
				 _channel = Hidden; 

				}
				break;

			}
			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("SkipSpace", 43);
			LeaveRule("SkipSpace", 43);
			Leave_SkipSpace();
		}
	}
	// $ANTLR end "SkipSpace"

	partial void Enter_NewLine();
	partial void Leave_NewLine();

	// $ANTLR start "NewLine"
	[GrammarRule("NewLine")]
	private void mNewLine()
	{
		Enter_NewLine();
		EnterRule("NewLine", 44);
		TraceIn("NewLine", 44);
		try
		{
			// CssGrammer.g3:175:2: ( '\\n' | '\\r' | '\\u2028' | '\\u2029' )
			DebugEnterAlt(1);
			// CssGrammer.g3:
			{
			DebugLocation(175, 2);
			if (input.LA(1)=='\n'||input.LA(1)=='\r'||(input.LA(1)>='\u2028' && input.LA(1)<='\u2029'))
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("NewLine", 44);
			LeaveRule("NewLine", 44);
			Leave_NewLine();
		}
	}
	// $ANTLR end "NewLine"

	partial void Enter_WhiteSpace();
	partial void Leave_WhiteSpace();

	// $ANTLR start "WhiteSpace"
	[GrammarRule("WhiteSpace")]
	private void mWhiteSpace()
	{
		Enter_WhiteSpace();
		EnterRule("WhiteSpace", 45);
		TraceIn("WhiteSpace", 45);
		try
		{
			// CssGrammer.g3:182:2: ( ( '\\t' | '\\v' | '\\f' | ' ' | '\\u00A0' ) )
			DebugEnterAlt(1);
			// CssGrammer.g3:
			{
			DebugLocation(182, 2);
			if (input.LA(1)=='\t'||input.LA(1)=='\f'||input.LA(1)==' '||input.LA(1)=='v'||input.LA(1)=='\u00A0')
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("WhiteSpace", 45);
			LeaveRule("WhiteSpace", 45);
			Leave_WhiteSpace();
		}
	}
	// $ANTLR end "WhiteSpace"

	partial void Enter_HexDigits();
	partial void Leave_HexDigits();

	// $ANTLR start "HexDigits"
	[GrammarRule("HexDigits")]
	private void mHexDigits()
	{
		Enter_HexDigits();
		EnterRule("HexDigits", 46);
		TraceIn("HexDigits", 46);
		try
		{
			// CssGrammer.g3:186:2: ( ( 'a' .. 'f' ) | ( '0' .. '9' ) )
			DebugEnterAlt(1);
			// CssGrammer.g3:
			{
			DebugLocation(186, 2);
			if ((input.LA(1)>='0' && input.LA(1)<='9')||(input.LA(1)>='a' && input.LA(1)<='f'))
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("HexDigits", 46);
			LeaveRule("HexDigits", 46);
			Leave_HexDigits();
		}
	}
	// $ANTLR end "HexDigits"

	partial void Enter_DecimalDigits();
	partial void Leave_DecimalDigits();

	// $ANTLR start "DecimalDigits"
	[GrammarRule("DecimalDigits")]
	private void mDecimalDigits()
	{
		Enter_DecimalDigits();
		EnterRule("DecimalDigits", 47);
		TraceIn("DecimalDigits", 47);
		try
		{
			// CssGrammer.g3:191:2: ( ( '0' .. '9' ) )
			DebugEnterAlt(1);
			// CssGrammer.g3:
			{
			DebugLocation(191, 2);
			if ((input.LA(1)>='0' && input.LA(1)<='9'))
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("DecimalDigits", 47);
			LeaveRule("DecimalDigits", 47);
			Leave_DecimalDigits();
		}
	}
	// $ANTLR end "DecimalDigits"

	partial void Enter_Alphabets();
	partial void Leave_Alphabets();

	// $ANTLR start "Alphabets"
	[GrammarRule("Alphabets")]
	private void mAlphabets()
	{
		Enter_Alphabets();
		EnterRule("Alphabets", 48);
		TraceIn("Alphabets", 48);
		try
		{
			// CssGrammer.g3:195:2: ( ( 'a' .. 'z' ) | ( 'A' .. 'Z' ) )
			DebugEnterAlt(1);
			// CssGrammer.g3:
			{
			DebugLocation(195, 2);
			if ((input.LA(1)>='A' && input.LA(1)<='Z')||(input.LA(1)>='a' && input.LA(1)<='z'))
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("Alphabets", 48);
			LeaveRule("Alphabets", 48);
			Leave_Alphabets();
		}
	}
	// $ANTLR end "Alphabets"

	public override void mTokens()
	{
		// CssGrammer.g3:1:8: ( T__33 | T__34 | T__35 | T__36 | T__37 | T__38 | T__39 | T__40 | T__41 | T__42 | T__43 | T__44 | T__45 | T__46 | T__47 | T__48 | T__49 | T__50 | T__51 | T__52 | T__53 | T__54 | T__55 | T__56 | T__57 | T__58 | T__59 | T__60 | T__61 | T__62 | T__63 | T__64 | T__65 | T__66 | T__67 | T__68 | IDENT | STRING | NUM | COLOR | SingleLineComment | Comment | SkipSpace )
		int alt20=43;
		try { DebugEnterDecision(20, decisionCanBacktrack[20]);
		try
		{
			alt20 = dfa20.Predict(input);
		}
		catch (NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
			throw;
		}
		} finally { DebugExitDecision(20); }
		switch (alt20)
		{
		case 1:
			DebugEnterAlt(1);
			// CssGrammer.g3:1:10: T__33
			{
			DebugLocation(1, 10);
			mT__33(); 

			}
			break;
		case 2:
			DebugEnterAlt(2);
			// CssGrammer.g3:1:16: T__34
			{
			DebugLocation(1, 16);
			mT__34(); 

			}
			break;
		case 3:
			DebugEnterAlt(3);
			// CssGrammer.g3:1:22: T__35
			{
			DebugLocation(1, 22);
			mT__35(); 

			}
			break;
		case 4:
			DebugEnterAlt(4);
			// CssGrammer.g3:1:28: T__36
			{
			DebugLocation(1, 28);
			mT__36(); 

			}
			break;
		case 5:
			DebugEnterAlt(5);
			// CssGrammer.g3:1:34: T__37
			{
			DebugLocation(1, 34);
			mT__37(); 

			}
			break;
		case 6:
			DebugEnterAlt(6);
			// CssGrammer.g3:1:40: T__38
			{
			DebugLocation(1, 40);
			mT__38(); 

			}
			break;
		case 7:
			DebugEnterAlt(7);
			// CssGrammer.g3:1:46: T__39
			{
			DebugLocation(1, 46);
			mT__39(); 

			}
			break;
		case 8:
			DebugEnterAlt(8);
			// CssGrammer.g3:1:52: T__40
			{
			DebugLocation(1, 52);
			mT__40(); 

			}
			break;
		case 9:
			DebugEnterAlt(9);
			// CssGrammer.g3:1:58: T__41
			{
			DebugLocation(1, 58);
			mT__41(); 

			}
			break;
		case 10:
			DebugEnterAlt(10);
			// CssGrammer.g3:1:64: T__42
			{
			DebugLocation(1, 64);
			mT__42(); 

			}
			break;
		case 11:
			DebugEnterAlt(11);
			// CssGrammer.g3:1:70: T__43
			{
			DebugLocation(1, 70);
			mT__43(); 

			}
			break;
		case 12:
			DebugEnterAlt(12);
			// CssGrammer.g3:1:76: T__44
			{
			DebugLocation(1, 76);
			mT__44(); 

			}
			break;
		case 13:
			DebugEnterAlt(13);
			// CssGrammer.g3:1:82: T__45
			{
			DebugLocation(1, 82);
			mT__45(); 

			}
			break;
		case 14:
			DebugEnterAlt(14);
			// CssGrammer.g3:1:88: T__46
			{
			DebugLocation(1, 88);
			mT__46(); 

			}
			break;
		case 15:
			DebugEnterAlt(15);
			// CssGrammer.g3:1:94: T__47
			{
			DebugLocation(1, 94);
			mT__47(); 

			}
			break;
		case 16:
			DebugEnterAlt(16);
			// CssGrammer.g3:1:100: T__48
			{
			DebugLocation(1, 100);
			mT__48(); 

			}
			break;
		case 17:
			DebugEnterAlt(17);
			// CssGrammer.g3:1:106: T__49
			{
			DebugLocation(1, 106);
			mT__49(); 

			}
			break;
		case 18:
			DebugEnterAlt(18);
			// CssGrammer.g3:1:112: T__50
			{
			DebugLocation(1, 112);
			mT__50(); 

			}
			break;
		case 19:
			DebugEnterAlt(19);
			// CssGrammer.g3:1:118: T__51
			{
			DebugLocation(1, 118);
			mT__51(); 

			}
			break;
		case 20:
			DebugEnterAlt(20);
			// CssGrammer.g3:1:124: T__52
			{
			DebugLocation(1, 124);
			mT__52(); 

			}
			break;
		case 21:
			DebugEnterAlt(21);
			// CssGrammer.g3:1:130: T__53
			{
			DebugLocation(1, 130);
			mT__53(); 

			}
			break;
		case 22:
			DebugEnterAlt(22);
			// CssGrammer.g3:1:136: T__54
			{
			DebugLocation(1, 136);
			mT__54(); 

			}
			break;
		case 23:
			DebugEnterAlt(23);
			// CssGrammer.g3:1:142: T__55
			{
			DebugLocation(1, 142);
			mT__55(); 

			}
			break;
		case 24:
			DebugEnterAlt(24);
			// CssGrammer.g3:1:148: T__56
			{
			DebugLocation(1, 148);
			mT__56(); 

			}
			break;
		case 25:
			DebugEnterAlt(25);
			// CssGrammer.g3:1:154: T__57
			{
			DebugLocation(1, 154);
			mT__57(); 

			}
			break;
		case 26:
			DebugEnterAlt(26);
			// CssGrammer.g3:1:160: T__58
			{
			DebugLocation(1, 160);
			mT__58(); 

			}
			break;
		case 27:
			DebugEnterAlt(27);
			// CssGrammer.g3:1:166: T__59
			{
			DebugLocation(1, 166);
			mT__59(); 

			}
			break;
		case 28:
			DebugEnterAlt(28);
			// CssGrammer.g3:1:172: T__60
			{
			DebugLocation(1, 172);
			mT__60(); 

			}
			break;
		case 29:
			DebugEnterAlt(29);
			// CssGrammer.g3:1:178: T__61
			{
			DebugLocation(1, 178);
			mT__61(); 

			}
			break;
		case 30:
			DebugEnterAlt(30);
			// CssGrammer.g3:1:184: T__62
			{
			DebugLocation(1, 184);
			mT__62(); 

			}
			break;
		case 31:
			DebugEnterAlt(31);
			// CssGrammer.g3:1:190: T__63
			{
			DebugLocation(1, 190);
			mT__63(); 

			}
			break;
		case 32:
			DebugEnterAlt(32);
			// CssGrammer.g3:1:196: T__64
			{
			DebugLocation(1, 196);
			mT__64(); 

			}
			break;
		case 33:
			DebugEnterAlt(33);
			// CssGrammer.g3:1:202: T__65
			{
			DebugLocation(1, 202);
			mT__65(); 

			}
			break;
		case 34:
			DebugEnterAlt(34);
			// CssGrammer.g3:1:208: T__66
			{
			DebugLocation(1, 208);
			mT__66(); 

			}
			break;
		case 35:
			DebugEnterAlt(35);
			// CssGrammer.g3:1:214: T__67
			{
			DebugLocation(1, 214);
			mT__67(); 

			}
			break;
		case 36:
			DebugEnterAlt(36);
			// CssGrammer.g3:1:220: T__68
			{
			DebugLocation(1, 220);
			mT__68(); 

			}
			break;
		case 37:
			DebugEnterAlt(37);
			// CssGrammer.g3:1:226: IDENT
			{
			DebugLocation(1, 226);
			mIDENT(); 

			}
			break;
		case 38:
			DebugEnterAlt(38);
			// CssGrammer.g3:1:232: STRING
			{
			DebugLocation(1, 232);
			mSTRING(); 

			}
			break;
		case 39:
			DebugEnterAlt(39);
			// CssGrammer.g3:1:239: NUM
			{
			DebugLocation(1, 239);
			mNUM(); 

			}
			break;
		case 40:
			DebugEnterAlt(40);
			// CssGrammer.g3:1:243: COLOR
			{
			DebugLocation(1, 243);
			mCOLOR(); 

			}
			break;
		case 41:
			DebugEnterAlt(41);
			// CssGrammer.g3:1:249: SingleLineComment
			{
			DebugLocation(1, 249);
			mSingleLineComment(); 

			}
			break;
		case 42:
			DebugEnterAlt(42);
			// CssGrammer.g3:1:267: Comment
			{
			DebugLocation(1, 267);
			mComment(); 

			}
			break;
		case 43:
			DebugEnterAlt(43);
			// CssGrammer.g3:1:275: SkipSpace
			{
			DebugLocation(1, 275);
			mSkipSpace(); 

			}
			break;

		}

	}


	#region DFA
	DFA8 dfa8;
	DFA11 dfa11;
	DFA20 dfa20;

	protected override void InitDFAs()
	{
		base.InitDFAs();
		dfa8 = new DFA8(this);
		dfa11 = new DFA11(this);
		dfa20 = new DFA20(this);
	}

	private class DFA8 : DFA
	{
		private const string DFA8_eotS =
			"\x1\xFFFF\x1\x3\x2\xFFFF";
		private const string DFA8_eofS =
			"\x4\xFFFF";
		private const string DFA8_minS =
			"\x2\x2E\x2\xFFFF";
		private const string DFA8_maxS =
			"\x2\x39\x2\xFFFF";
		private const string DFA8_acceptS =
			"\x2\xFFFF\x1\x1\x1\x2";
		private const string DFA8_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA8_transitionS =
			{
				"\x1\x2\x1\xFFFF\xA\x1",
				"\x1\x2\x1\xFFFF\xA\x1",
				"",
				""
			};

		private static readonly short[] DFA8_eot = DFA.UnpackEncodedString(DFA8_eotS);
		private static readonly short[] DFA8_eof = DFA.UnpackEncodedString(DFA8_eofS);
		private static readonly char[] DFA8_min = DFA.UnpackEncodedStringToUnsignedChars(DFA8_minS);
		private static readonly char[] DFA8_max = DFA.UnpackEncodedStringToUnsignedChars(DFA8_maxS);
		private static readonly short[] DFA8_accept = DFA.UnpackEncodedString(DFA8_acceptS);
		private static readonly short[] DFA8_special = DFA.UnpackEncodedString(DFA8_specialS);
		private static readonly short[][] DFA8_transition;

		static DFA8()
		{
			int numStates = DFA8_transitionS.Length;
			DFA8_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA8_transition[i] = DFA.UnpackEncodedString(DFA8_transitionS[i]);
			}
		}

		public DFA8( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 8;
			this.eot = DFA8_eot;
			this.eof = DFA8_eof;
			this.min = DFA8_min;
			this.max = DFA8_max;
			this.accept = DFA8_accept;
			this.special = DFA8_special;
			this.transition = DFA8_transition;
		}

		public override string Description { get { return "146:8: ( ( DecimalDigits )* '.' )?"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA11 : DFA
	{
		private const string DFA11_eotS =
			"\x1\xFFFF\x1\x3\x2\xFFFF";
		private const string DFA11_eofS =
			"\x4\xFFFF";
		private const string DFA11_minS =
			"\x2\x2E\x2\xFFFF";
		private const string DFA11_maxS =
			"\x2\x39\x2\xFFFF";
		private const string DFA11_acceptS =
			"\x2\xFFFF\x1\x1\x1\x2";
		private const string DFA11_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA11_transitionS =
			{
				"\x1\x2\x1\xFFFF\xA\x1",
				"\x1\x2\x1\xFFFF\xA\x1",
				"",
				""
			};

		private static readonly short[] DFA11_eot = DFA.UnpackEncodedString(DFA11_eotS);
		private static readonly short[] DFA11_eof = DFA.UnpackEncodedString(DFA11_eofS);
		private static readonly char[] DFA11_min = DFA.UnpackEncodedStringToUnsignedChars(DFA11_minS);
		private static readonly char[] DFA11_max = DFA.UnpackEncodedStringToUnsignedChars(DFA11_maxS);
		private static readonly short[] DFA11_accept = DFA.UnpackEncodedString(DFA11_acceptS);
		private static readonly short[] DFA11_special = DFA.UnpackEncodedString(DFA11_specialS);
		private static readonly short[][] DFA11_transition;

		static DFA11()
		{
			int numStates = DFA11_transitionS.Length;
			DFA11_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA11_transition[i] = DFA.UnpackEncodedString(DFA11_transitionS[i]);
			}
		}

		public DFA11( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 11;
			this.eot = DFA11_eot;
			this.eof = DFA11_eof;
			this.min = DFA11_min;
			this.max = DFA11_max;
			this.accept = DFA11_accept;
			this.special = DFA11_special;
			this.transition = DFA11_transition;
		}

		public override string Description { get { return "147:4: ( ( DecimalDigits )* '.' )?"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA20 : DFA
	{
		private const string DFA20_eotS =
			"\x1\xFFFF\x1\x26\x5\xFFFF\x1\x28\x1\x2A\x3\xFFFF\x1\x2C\x2\xFFFF\xA\x25"+
			"\x1\x3B\x13\xFFFF\x1\x40\x1\x25\x1\x42\x1\x43\x1\x25\x1\x45\x1\x46\x1"+
			"\x25\x1\x48\x1\x49\x1\x4A\x1\x4B\x1\x4C\x1\x25\x6\xFFFF\x1\x4E\x2\xFFFF"+
			"\x1\x25\x2\xFFFF\x1\x50\x5\xFFFF\x1\x51\x1\xFFFF\x1\x52\x3\xFFFF";
		private const string DFA20_eofS =
			"\x53\xFFFF";
		private const string DFA20_minS =
			"\x1\x9\x1\x30\x5\xFFFF\x1\x30\x1\x3A\x3\xFFFF\x1\x69\x2\xFFFF\x1\x6D"+
			"\x1\x65\x1\x6D\x1\x72\x1\x7A\x1\x6E\x1\x68\x1\x6D\x1\x63\x1\x61\x1\x2D"+
			"\x5\xFFFF\x1\x2E\x2\xFFFF\x1\x2A\x8\xFFFF\x1\x6D\x1\xFFFF\x1\x2D\x1\x67"+
			"\x2\x2D\x1\x61\x2\x2D\x1\x7A\x5\x2D\x1\x64\x6\xFFFF\x1\x2D\x2\xFFFF\x1"+
			"\x64\x2\xFFFF\x1\x2D\x5\xFFFF\x1\x2D\x1\xFFFF\x1\x2D\x3\xFFFF";
		private const string DFA20_maxS =
			"\x1\xFFFE\x1\x66\x5\xFFFF\x1\x39\x1\x3A\x3\xFFFF\x1\x69\x2\xFFFF\x1\x6D"+
			"\x1\x65\x1\x78\x1\x72\x1\x7A\x1\x6E\x1\x68\x1\x73\x1\x78\x1\x61\x1\xFFFE"+
			"\x5\xFFFF\x1\xFFFE\x2\xFFFF\x1\x2F\x8\xFFFF\x1\x6E\x1\xFFFF\x1\xFFFE"+
			"\x1\x67\x2\xFFFE\x1\x61\x2\xFFFE\x1\x7A\x5\xFFFE\x1\x64\x6\xFFFF\x1\xFFFE"+
			"\x2\xFFFF\x1\x64\x2\xFFFF\x1\xFFFE\x5\xFFFF\x1\xFFFE\x1\xFFFF\x1\xFFFE"+
			"\x3\xFFFF";
		private const string DFA20_acceptS =
			"\x2\xFFFF\x1\x2\x1\x3\x1\x4\x1\x5\x1\x6\x2\xFFFF\x1\xA\x1\xB\x1\xC\x1"+
			"\xFFFF\x1\x10\x1\x11\xB\xFFFF\x1\x21\x1\x22\x1\x23\x1\x24\x1\x25\x1\xFFFF"+
			"\x1\x26\x1\x27\x1\xFFFF\x1\x25\x1\x2B\x1\x25\x1\x1\x1\x28\x1\x7\x1\x9"+
			"\x1\x8\x1\xFFFF\x1\xD\xE\xFFFF\x1\x20\x1\x29\x1\x2A\x1\xE\x1\xF\x1\x12"+
			"\x1\xFFFF\x1\x14\x1\x15\x1\xFFFF\x1\x17\x1\x18\x1\xFFFF\x1\x1A\x1\x1B"+
			"\x1\x1C\x1\x1D\x1\x1E\x1\xFFFF\x1\x13\x1\xFFFF\x1\x19\x1\x1F\x1\x16";
		private const string DFA20_specialS =
			"\x53\xFFFF}>";
		private static readonly string[] DFA20_transitionS =
			{
				"\x2\x24\x1\xFFFF\x2\x24\x12\xFFFF\x1\x24\x1\xFFFF\x1\x20\x1\x1\x1\xFFFF"+
				"\x1\x2\x1\xFFFF\x1\x20\x1\x3\x1\x4\x1\xFFFF\x1\x5\x1\x6\x1\x1F\x1\x7"+
				"\x1\x22\xA\x21\x1\x8\x1\x9\x1\xFFFF\x1\xA\x1\xB\x1\xFFFF\x1\xC\x1A\x25"+
				"\x1\xD\x1\xFFFF\x1\xE\x1\xFFFF\x1\x25\x1\xFFFF\x2\x25\x1\xF\x1\x10\x1"+
				"\x11\x1\x25\x1\x12\x1\x13\x1\x14\x1\x25\x1\x15\x1\x25\x1\x16\x2\x25"+
				"\x1\x17\x1\x25\x1\x18\x1\x19\x2\x25\x1\x23\x4\x25\x1\x1A\x1\x1B\x1\x1C"+
				"\x1\x1D\x21\xFFFF\x1\x24\x5F\xFFFF\x1F28\x25\x2\x1E\xDFD5\x25",
				"\xA\x27\x27\xFFFF\x6\x27",
				"",
				"",
				"",
				"",
				"",
				"\xA\x21",
				"\x1\x29",
				"",
				"",
				"",
				"\x1\x2B",
				"",
				"",
				"\x1\x2D",
				"\x1\x2E",
				"\x1\x2F\xA\xFFFF\x1\x30",
				"\x1\x31",
				"\x1\x32",
				"\x1\x33",
				"\x1\x34",
				"\x1\x35\x5\xFFFF\x1\x36",
				"\x1\x37\x10\xFFFF\x1\x38\x3\xFFFF\x1\x39",
				"\x1\x3A",
				"\x1\x25\x2\xFFFF\xA\x25\x7\xFFFF\x1A\x25\x4\xFFFF\x1\x25\x1\xFFFF\x1A"+
				"\x25\x85\xFFFF\xFEFF\x25",
				"",
				"",
				"",
				"",
				"",
				"\x1\x21\x1\xFFFF\xA\x21\x7\xFFFF\x1A\x25\x4\xFFFF\x1\x25\x1\xFFFF\x1A"+
				"\x25\x85\xFFFF\xFEFF\x25",
				"",
				"",
				"\x1\x3D\x4\xFFFF\x1\x3C",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"\x1\x3E\x1\x3F",
				"",
				"\x1\x25\x2\xFFFF\xA\x25\x7\xFFFF\x1A\x25\x4\xFFFF\x1\x25\x1\xFFFF\x1A"+
				"\x25\x85\xFFFF\xFEFF\x25",
				"\x1\x41",
				"\x1\x25\x2\xFFFF\xA\x25\x7\xFFFF\x1A\x25\x4\xFFFF\x1\x25\x1\xFFFF\x1A"+
				"\x25\x85\xFFFF\xFEFF\x25",
				"\x1\x25\x2\xFFFF\xA\x25\x7\xFFFF\x1A\x25\x4\xFFFF\x1\x25\x1\xFFFF\x1A"+
				"\x25\x85\xFFFF\xFEFF\x25",
				"\x1\x44",
				"\x1\x25\x2\xFFFF\xA\x25\x7\xFFFF\x1A\x25\x4\xFFFF\x1\x25\x1\xFFFF\x1A"+
				"\x25\x85\xFFFF\xFEFF\x25",
				"\x1\x25\x2\xFFFF\xA\x25\x7\xFFFF\x1A\x25\x4\xFFFF\x1\x25\x1\xFFFF\x1A"+
				"\x25\x85\xFFFF\xFEFF\x25",
				"\x1\x47",
				"\x1\x25\x2\xFFFF\xA\x25\x7\xFFFF\x1A\x25\x4\xFFFF\x1\x25\x1\xFFFF\x1A"+
				"\x25\x85\xFFFF\xFEFF\x25",
				"\x1\x25\x2\xFFFF\xA\x25\x7\xFFFF\x1A\x25\x4\xFFFF\x1\x25\x1\xFFFF\x1A"+
				"\x25\x85\xFFFF\xFEFF\x25",
				"\x1\x25\x2\xFFFF\xA\x25\x7\xFFFF\x1A\x25\x4\xFFFF\x1\x25\x1\xFFFF\x1A"+
				"\x25\x85\xFFFF\xFEFF\x25",
				"\x1\x25\x2\xFFFF\xA\x25\x7\xFFFF\x1A\x25\x4\xFFFF\x1\x25\x1\xFFFF\x1A"+
				"\x25\x85\xFFFF\xFEFF\x25",
				"\x1\x25\x2\xFFFF\xA\x25\x7\xFFFF\x1A\x25\x4\xFFFF\x1\x25\x1\xFFFF\x1A"+
				"\x25\x85\xFFFF\xFEFF\x25",
				"\x1\x4D",
				"",
				"",
				"",
				"",
				"",
				"",
				"\x1\x25\x2\xFFFF\xA\x25\x7\xFFFF\x1A\x25\x4\xFFFF\x1\x25\x1\xFFFF\x1A"+
				"\x25\x85\xFFFF\xFEFF\x25",
				"",
				"",
				"\x1\x4F",
				"",
				"",
				"\x1\x25\x2\xFFFF\xA\x25\x7\xFFFF\x1A\x25\x4\xFFFF\x1\x25\x1\xFFFF\x1A"+
				"\x25\x85\xFFFF\xFEFF\x25",
				"",
				"",
				"",
				"",
				"",
				"\x1\x25\x2\xFFFF\xA\x25\x7\xFFFF\x1A\x25\x4\xFFFF\x1\x25\x1\xFFFF\x1A"+
				"\x25\x85\xFFFF\xFEFF\x25",
				"",
				"\x1\x25\x2\xFFFF\xA\x25\x7\xFFFF\x1A\x25\x4\xFFFF\x1\x25\x1\xFFFF\x1A"+
				"\x25\x85\xFFFF\xFEFF\x25",
				"",
				"",
				""
			};

		private static readonly short[] DFA20_eot = DFA.UnpackEncodedString(DFA20_eotS);
		private static readonly short[] DFA20_eof = DFA.UnpackEncodedString(DFA20_eofS);
		private static readonly char[] DFA20_min = DFA.UnpackEncodedStringToUnsignedChars(DFA20_minS);
		private static readonly char[] DFA20_max = DFA.UnpackEncodedStringToUnsignedChars(DFA20_maxS);
		private static readonly short[] DFA20_accept = DFA.UnpackEncodedString(DFA20_acceptS);
		private static readonly short[] DFA20_special = DFA.UnpackEncodedString(DFA20_specialS);
		private static readonly short[][] DFA20_transition;

		static DFA20()
		{
			int numStates = DFA20_transitionS.Length;
			DFA20_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA20_transition[i] = DFA.UnpackEncodedString(DFA20_transitionS[i]);
			}
		}

		public DFA20( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 20;
			this.eot = DFA20_eot;
			this.eof = DFA20_eof;
			this.min = DFA20_min;
			this.max = DFA20_max;
			this.accept = DFA20_accept;
			this.special = DFA20_special;
			this.transition = DFA20_transition;
		}

		public override string Description { get { return "1:1: Tokens : ( T__33 | T__34 | T__35 | T__36 | T__37 | T__38 | T__39 | T__40 | T__41 | T__42 | T__43 | T__44 | T__45 | T__46 | T__47 | T__48 | T__49 | T__50 | T__51 | T__52 | T__53 | T__54 | T__55 | T__56 | T__57 | T__58 | T__59 | T__60 | T__61 | T__62 | T__63 | T__64 | T__65 | T__66 | T__67 | T__68 | IDENT | STRING | NUM | COLOR | SingleLineComment | Comment | SkipSpace );"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

 
	#endregion

}

} // namespace CssParser
