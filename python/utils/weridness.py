import re

def fix_weridness_for_strings(input: str):
    """
    This is a hackish workaround for something I can't figure out.
    """
    # check for more than 1 sourrending paranthesis around a string using regex
    match = re.search(r"\(\(.*\)\)", input)
    if match:
        # Get the content of the paranthesis
        string = match.group(0)
        # Remove all paranthesis
        string = string.replace("(", "")
        string = string.replace(")", "")
        if string == "None":
            # Replace the string with the original string
            input = input.replace(match.group(0), "None")
        
        else:
            # wrap the string in paranthesis
            string = f"({string})"
            # Replace the string with the original string
            input = input.replace(match.group(0), string)

    # Check for (None) and replace instances of it with None
    input = input.replace("(None)", "None")
    
    # check repeating text like (npc) (npc) (npc)
    matches = re.findall(r"\(.*\)\s\(.*\)\"", input)
    # there may be multiple matches, we have to deal with them all
    if len(matches) > 0:
        for match in matches:
            # split the match by spaces
            split = match.split(" ")
            # get the last item in the list
            string = split[-1]
            # replace the input with the new string
            input = input.replace(match, string)
            # print(split, string, input)
    

    # if there isn't anything we need to fix, just return the input
    return input
