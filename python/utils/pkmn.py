# type: ignore ReportMissingImport

import json
from utils.helpers import LanguageStrings, MoveTypes, get_generation_from_version
from utils.sprites import Sprites
from utils.weridness import fix_weridness_for_strings
from utils.legality import legality_check
import base64, io
import segno
from PKHeX.Core import GameInfo, RibbonInfo, RibbonStrings, EntitySummary, GameStrings, QRMessageUtil, FormConverter

class Pokemon:
    def __init__(self, pkmn, strings: LanguageStrings, moveTypes: MoveTypes, spriteHelper: Sprites, generation: str) -> None:
        # We can piggyback off of EntitySummary to get a lot of the data we need
        entity = EntitySummary(pkmn, GameStrings("en"))
        self.nickname = entity.Nickname
        self.species = entity.Species
        self.nature = entity.Nature
        self.gender = entity.Gender
        self.esv = entity.ESV
        self.hp_type = entity.HP_Type
        self.ability = entity.Ability
        self.held_item = entity.HeldItem
        self.ball = entity.Ball
        self.ot = entity.OT
        self.version = entity.Version
        self.ot_lang = entity.OTLang
        self.ec = entity.EC
        self.pid = entity.PID
        self.exp = entity.EXP
        self.level = entity.Level
        self.markings = entity.Markings
        self.ht = entity.NotOT
        self.ability_num = entity.AbilityNum
        self.gender_flag = entity.GenderFlag
        self.form_num = entity.Form
        self.pkrs_strain = entity.PKRS_Strain
        self.pkrs_days = entity.PKRS_Days
        self.fateful_flag = entity.FatefulFlag
        self.is_egg = entity.IsEgg
        self.is_nicknamed = entity.IsNicknamed
        self.is_shiny = entity.IsShiny
        self.tid = entity.TID
        self.sid = entity.SID
        self.tsv = entity.TSV
        self.checksum = entity.Checksum
        self.friendship = entity.Friendship
        # Now for the custom ones that we do not rely on EntitySummary for
        self.relearn_moves = [
            self.Move(pkmn.RelearnMove1, None, None, strings, moveTypes),
            self.Move(pkmn.RelearnMove2, None, None, strings, moveTypes),
            self.Move(pkmn.RelearnMove3, None, None, strings, moveTypes),
            self.Move(pkmn.RelearnMove4, None, None, strings, moveTypes)
        ]
        self.contest_stats = [ # Technically we do use EntitySummary for this, but we need to convert it to a list
            self.ContestStat("Cool", entity.Cool),
            self.ContestStat("Beauty", entity.Beauty),
            self.ContestStat("Cute", entity.Cute),
            self.ContestStat("Smart", entity.Smart),
            self.ContestStat("Tough", entity.Tough),
            self.ContestStat("Sheen", entity.Sheen)
        ]
        self.stats = [ # Technically we do use EntitySummary for this, but we need to convert it to a list
            self.Stat("HP", entity.HP_EV, entity.HP_IV, entity.HP),
            self.Stat("Attack", entity.ATK_EV, entity.ATK_IV, entity.ATK),
            self.Stat("Defense", entity.DEF_EV, entity.DEF_IV, entity.DEF),
            self.Stat("Special Attack", entity.SPA_EV, entity.SPA_IV, entity.SPA),
            self.Stat("Special Defense", entity.SPD_EV, entity.SPD_IV, entity.SPD),
            self.Stat("Speed", entity.SPE_EV, entity.SPE_IV, entity.SPE),
        ]
        self.moves = [
            self.Move(pkmn.Move1, pkmn.Move1_PP, pkmn.Move1_PPUps, strings, moveTypes),
            self.Move(pkmn.Move2, pkmn.Move2_PP, pkmn.Move2_PPUps, strings, moveTypes),
            self.Move(pkmn.Move3, pkmn.Move3_PP, pkmn.Move3_PPUps, strings, moveTypes),
            self.Move(pkmn.Move4, pkmn.Move4_PP, pkmn.Move4_PPUps, strings, moveTypes)
        ]
        self.met_data = self.MetData(entity.MetLoc, entity.MetLevel, entity.Met_Year, entity.Met_Month, entity.Met_Day)
        self.egg_data = self.EggData(entity.EggLoc, entity.Egg_Year, entity.Egg_Month, entity.Egg_Day)

        self.ot_gender = GameInfo.GenderSymbolASCII[pkmn.OT_Gender]
        self.is_legal, self.illegal_reasons = legality_check(pkmn)
        try:
            self.generation = get_generation_from_version(pkmn.Version)
        except:

            self.generation = generation if generation != "" else str(pkmn.Generation)

        self.dex_number = pkmn.Species
        self.size = pkmn.SIZE_STORED
        self.item_num = pkmn.HeldItem
        self.ribbons = [RibbonStrings.GetName(ribbon.Name) for ribbon in RibbonInfo.GetRibbonInfo(pkmn) if ribbon.HasRibbon]
        self.base64 = base64.b64encode(bytearray(byte for byte in pkmn.DecryptedBoxData)).decode('UTF-8')
        #self.qr = self.genQR(QRMessageUtil.GetMessage(pkmn))
        self.sprites = self.Sprites(spriteHelper, 
                                    entity.Species, 
                                    pkmn.Species, "female" if entity.Gender == 'F' else  "male", 
                                    entity.IsShiny, 
                                    entity.Form, 
                                    pkmn.Generation, 
                                    pkmn.Context,
                                    pkmn.FormArgument if hasattr(pkmn, "FormArgument") else 0,
                                    pkmn.HeldItem
                                    )          

    def toJSON(self):
        return fix_weridness_for_strings(json.dumps(self, default=lambda o: o.__dict__, 
            sort_keys=True, indent=4, ensure_ascii=False))

    def genQR(self, data):
        buff = io.BytesIO()
        segno.make_qr(data).save(buff, kind='png')
        qr = base64.b64encode(buff.getvalue()).decode('UTF-8')
        buff.close()
        return qr

    class Move:
        def __init__(self, move, pp, pp_ups, strings: LanguageStrings, moveTypes: MoveTypes) -> None:
            self.name = "None" if move == 0 else strings.get_move_name(move)
            self.type = strings.get_type_name(moveTypes.get_type_name(str(move)))
            if pp is not None and pp_ups is not None:
                self.pp = pp
                self.pp_ups = pp_ups

    class Stat:
        def __init__(self, name, iv, ev, total) -> None:
            self.stat_name = name
            self.stat_iv = iv
            self.stat_ev = ev
            self.stat_total = total

    class ContestStat:
        def __init__(self, name, value) -> None:
            self.stat_name = name
            self.stat_value = value

    class MetData:
        def __init__(self, met_loc, met_level, met_year, met_month, met_day) -> None:
            self.location = met_loc
            self.level = met_level
            self.year = met_year
            self.month = met_month
            self.day = met_day

    class EggData:
        def __init__(self, egg_location, egg_year, egg_month, egg_day) -> None:
            self.location = egg_location
            self.year = egg_year
            self.month = egg_month
            self.day = egg_day
    class Sprites:
        def __init__(self, spriteHelper: Sprites, species: str, speciesNum: int, gender: str, shiny: bool, form: int, generation: int, context, formArg, item) -> None:
            formName = self._get_form_name(speciesNum, form, context)
            self.species = spriteHelper.get_pokemon_sprite(species.lower(), gender, shiny, formName.lower(), generation >= 8, formArg)
            self.item = None if item == 0 else spriteHelper.get_item_sprite(item)
        
        def _get_form_name(self, species, form, context):
            formList = FormConverter.GetFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms, GameInfo.GenderSymbolUnicode, context)
            if len(formList) > 1:
                return formList[form]
            return formList[0]
