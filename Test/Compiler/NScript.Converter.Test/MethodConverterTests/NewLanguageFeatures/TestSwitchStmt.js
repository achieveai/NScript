function NewLanguageFeatures__SumPositiveNumbers(sequence) {
    var sum, i;
    for (i = sequence.GetEnumerable(); i != sequence.End(); i = sequence.Next()) {
        switch (i)
        {
            case 0:
            case 10:
                break;
            case (i.GetType() == IEnumerable$i32.id):
                {

                }
            case (i.GetType() == Number.id && (Cast(Number, i) > 0)):
                {
                    var n;
                    n = Cast(Number, i);
                    sum += n;
                }
            case null:
                throw new ArgumentException("Null not found");
            default:
                throw new InvalidProgramException("UnrecognizedType");
        }
    }
    return sum;
}