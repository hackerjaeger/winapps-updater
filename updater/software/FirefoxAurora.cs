﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017 - 2025  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Developer Edition (i.e. aurora channel)
    /// </summary>
    public class FirefoxAurora : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxAurora class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox Aurora
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "135.0b9";


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/135.0b9/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "f612abb0ae4098a50ce2e53fcc0e6da928f8eaf0b20d4775b105f10a8450d643334e991a312a682c1adcd93f73a3c9f9fb72bc3446e923931ed4d124d5f3fb13" },
                { "af", "26fce65c2a54125db7800e8d8d4d1dfe12150c4665b2134204de60455108912c91c538c54e2d27c73415e7c7685e83270facebdf1ea497f299bdab747693e408" },
                { "an", "680e47c3f18587c71d44d53e05c2941e0dd75a3fd2f31c67f57a1095909fb18d6de35a905e066ad739889f08ec2ce1b2cce7c67321d5b28fbd3f82bfcc1521e2" },
                { "ar", "692f60bde2f121056041c1815668477ab7820b24180e7d8141c903786693037c68568f3bc4938497909c5b25e98ff98dcf118525535ea59c3de8db94c3975856" },
                { "ast", "f45d6156afd174ce282a590fd322b0809d1920bd78764e9400b46b793ab39357a7aa1ddc9eea5649486e252d3344ed92723847d1cc7fe17d94fada6c9330f7f1" },
                { "az", "ad79aabbd9debd01e10321f0d6c1e0a5d1e8c26935311e59e4a9862b567bb896dc26fe02af44c174dfa9880f1d388cd9a7408da3dae9c67f14ae5b3d8a0dde51" },
                { "be", "3364c160692f53c48593d0d8bbffad59a232d55e7663ce397852e791ed06af9f0e975b91dffedc8b9f0762ad769311bb0b5b4c14f8fb64381dde0a35598d2d00" },
                { "bg", "343f8f887b393319f77b00cfb92fd56e9ef9d359d34535016502c63f2bb170ec715ee473214edb050b37302bd0c492ae2205caa04603f964f21c26f7309597ca" },
                { "bn", "23082c84dfadd2821f07026f068d41f3c7cc2e682d0d6b217ad2596402c124bdcce6f2b212f5044245f7f3c4f390670452b94032ca983e4368f74340860714f2" },
                { "br", "649077df6717242cdc96ff80656ee08248f41d81890ba62ba62925dd17ad4bb73ee1d29b843437505cdfbd77baa6f807e6c975202aef7762c8b4d336b20d24bf" },
                { "bs", "fe7f519d4de6ee1d928d5982ef45ea43a148fa3ce8d643307ee25d46977bfbe0002b1383e7431a0920eb0d50aea2c5804724edfc20ce16fa9d5a9ad8f1bcf6cc" },
                { "ca", "4bf5ae434dbeae91773a3bdf9518f2f6336c1d638053479eaf5cac173058093f99026c3987f808eac9de2f4c924f41e6aee5ff43a0c438d1a2924d291ac47f19" },
                { "cak", "a50fdad63e24a7106860d0b9bb7b7bed246e34951836090b6ec6a3367ab196102afc3a9fa22abeeebcb998653f37e31756604cf9f88c71b31a671d3aed2302c9" },
                { "cs", "34c71a53104e3b094f032f3890444c47378acd1082d01830c5a0f52bfcdc4be43605fe1e9b1b13bdfd16bc4cb2750c5c67e18bf1b92503578ab25ece693f7359" },
                { "cy", "3dcc9f0827128c02ae5d5920d64fac2fbe15431c6e43c6ff9ddd8e1833d8846c79242689e4690fbd67aedf4e61b2098604a6ec645554f032f4bc2b7759f73508" },
                { "da", "026b44261498c9d718a98b55852ecba61c7cd4286a9daf57c2a4a864b6614ad50df63ff91281db1dd9e92d76e11f97e06dcb1e7a5d90d30c404aff8b69893c80" },
                { "de", "4724d0c2f71bd959484e45d38d5dac301657d3715b0de03bd79d7d6cfdf106af7f7e4b5d012446b3227b4822ce55bdeaff48553c27afa5b0f2d54349ed945610" },
                { "dsb", "150c565269dce1b3537455687849720c606f281a97b391eb91cf37ecf42a81e6b5b96f4305ba4a4358db2b2b3070ffb190c27e170eced7d36644e54ff5281d2e" },
                { "el", "63173ab5fb60e9bf7e1dfe58d3d09d550068c6f312dc7131a8fb132b6f3700624b9aeffb1de91feca3f9c401fc2fbe489b5e7bd83c28fff09543e751ffae9202" },
                { "en-CA", "25d9650267363dff47a415168d4355c8bd2bb09cfa1ed5e7ba2c3dc78015e2d7a76397b0f71e2368cf61a53a0d7f403b026c23ce772ea682af917b0426f27a26" },
                { "en-GB", "6f7e040321d28fe02594728514b87f9fca0900137e58e304807b7fdb34608405a3c7de1eec713a73b10cd03defa8ce2ef00a8dcd3a24141b06193f98b6df07f8" },
                { "en-US", "4e9f95ba07d11359dee5ecd88a1b6ace2eda09efaacf138b6b4ff57f79ae3feee452aefdf58b0476ab54f68d721079e4b615029591d411f00770af93824dcad2" },
                { "eo", "5d7fb3c03cbec560cdae3e4ba97f43e981e621dadf01488570fcb9db2f19ac61319ec0be0ff19ffd656cf016dc87daf8deceed3383aa26d433d1affc18571b88" },
                { "es-AR", "eef15514165f8478d323f855f16afb40b5edc22ee0fa5e1aa684a6aa8993394aa0c24ae893825136e2a7446d6d33030d4ac579b503d450c91f1db6d6d2c44f69" },
                { "es-CL", "041ce1b1aef13085f5d834e859171badeb78ac59c75a96dc99225cabc5c8a089272538c1ec3fa7f4dee235f360f63334877c4e3a6441080f259b50dd3a937a06" },
                { "es-ES", "1fe9c68045fef2980f82e46df44558e8e9e6fb47313b7ebe8a45f6cdd6fa8b48c044b99050f206b391cfc9edb005d83901e8d2c6e30fa16eafda88097c2ea894" },
                { "es-MX", "bb468aa396242450fc9663c2c38ca9210ef0aa31998d1980a863b9ff2ec98751e519fa5957356616d96578fd59ca2dfff843a927f5488aea9a236122a8851709" },
                { "et", "66878220b554c17bc5744e6ef85a27ac2529c1ae8e1670024384ecb8a5f5372ece7c78abb3483fcaa8d30bfd99c7294b8b505b2546e933055e400490899333ec" },
                { "eu", "97268ca58e01290f2e161db23a0256bbe6ac530e1118b8fd756e27f8452af46b7561b9f497ac8d9335561fbb19773337f76f057117a41ef29970a89be6c4c811" },
                { "fa", "709abaf334c6b2f16d9544b4ca0551542f85c78eb6639ecfa55903acf04f1509f6911d3ab80abc0330b177a8a34805e7bf022843911faacec2830203cf3a8afc" },
                { "ff", "d7fed65dbdd47646edbabb7b1cabf0a353e961dce93a41f1c8e0a59021d52aec57a747373990ad540a5b423f0b23e73f7684b5a5930fd548295aac027156dc63" },
                { "fi", "9d523c0781f670fe9ae99de1847d3a371d99aea0e21592eb3881ac487f58877c22a1f50941252931d01fb79c42517562d60d45f0d2540eecb7b365ea104a803f" },
                { "fr", "bd4639f80e2861238b0bc77e1ca35572c1787b4243a13f98306b74326e0f0a4dc596f37155ad66cc8d9b130de50c45f9c3036518cd459a84c07531e8dfcbf2aa" },
                { "fur", "28892aa69efcd893d5bdff52430bf802eb6ca718491cddf2a981401463de2929c61417ae14ca93fcbbc93db8b47b42a7e3dcac2a09279933186cf026618aec7e" },
                { "fy-NL", "37b05f78629cebf4a12d9a4915bce1296e1ce9091bae3455286dbe164b5fd9291d316c942015b38b55acd1d97c2662d9bab904e78e2cf32952b4a3f5820070c3" },
                { "ga-IE", "79246b25fd57bb61f54ec4e550f3d612bb05f099655e30615bd42b1191327cca2d8b150f5f7d8cee521cb8d978a8dfa919495f2094377aed4e5838b8147370a2" },
                { "gd", "fc69a276d8152e43abcadf0117e2305ae3e65525cbff1d32c710cf6c3b192ef882df01c7918a9a4b57c2b34253ca4d3dc2ee6be420e4d11b4094c5f8788b76c1" },
                { "gl", "b240b42dc3db16627b35c88b16d5d47d610db4ea8ce3a5cae84a2eb6e783ece44654afa3e860964f54ad214b2b8046cc09d6a10506d00bf120276601c8573f5f" },
                { "gn", "0dd9b571a11f6ea471672e4b25548f457fd7cc73e9909a66061dd3ae01003566b9cde1a86abe4d6a3f323ee6b92c50e3396104734447dff12bcfb0e3e3eb1bf6" },
                { "gu-IN", "bcf1884fb91c0ba9d912be85dfdfcda95733554a3da0343ca99ff46b73f55a8b0f9129c4e74ea32f998c213300cccc48104ea1e2b6362351cb610b55886d940c" },
                { "he", "13b2b09d52ba65ddf4c617bee3311bc6bb06e13c54cb6c343af18e1b80869f03f29619118a1ac88e2f4174320e45e71319f2c8d932416fdb847b6abb77cb70e3" },
                { "hi-IN", "7f8c865af29416137e15bb8ea005a94247ea2e312e9219a9ecaa833cceb5018f7cd509e1e195f1bd27ed639c303d9857cc645bf0801d869e774fcce61c19b55d" },
                { "hr", "1b4fecebd0b2199b8e3c19c3bf76a311106ceaa5418e9e468e120e3d65b652b5149e1fd15bb8510a6cc1a53603f41305f54bcd0ce1d732e2e0232b5f5a3aaf71" },
                { "hsb", "09a333e37929688b27417a323fb93720288f8ad99a85018a89a39cf0127c31d6d9a2f250a9d2b76ae36aad451bfd6fdfdbe15ed6c3b30fc69193caf951c6bf97" },
                { "hu", "5dc08233724a383e04d3edfcf3fa71dd4b6423caed292152a9d38d5a550270040cbacf2a7e982f484a906568870ab36496c8fee8899fc006e2af0dae8be640a8" },
                { "hy-AM", "4cac0322893b5ba092bf6942a255a7e49f8ff5810703eb3d74f0900acf23bffc415d5930d3972bd32f38ca8be97c537c0526028b7f058a2eaa9b4c4ceb6b1a0f" },
                { "ia", "60c31f341df70b91ee3d2ee41d78cddd698adef7ecce755c625eebdbd0ec1d9183c1f80b6fce3aa82ba64e38a1403b0acdffb839ec94c9ec5ebade9b0bc1e439" },
                { "id", "2c19c563286b0d76b6aba4f884a8532b1118038049c53ee8107636268eb183c5b01c27dab818f611a59ad3762c32d00a18982338f7faa09b05789bb6c9267819" },
                { "is", "07e674aed667814b48ada8482b54f603f54fef55be106a9eb1791fc6a417f7fe8d50e771a546268f313b9efcfb8e6b21ab9f489f33371cf50f3a0eefc0eee596" },
                { "it", "70315946aec5818b2290c615a8ab9178c80e4d6ef76170f06eae7aa3dd6d0e477844fae2dcaae49a580b145885c9fbd8c3a0d5e040dae0d3cb6e6478ad5c46a7" },
                { "ja", "94dffe887d013b7c78f165891e0cb0de69fdb93adf307636c9eefae75d8c25fa3975686e808c0a1323fa92b053cccd62dd19749aae860b3ebb0c767eee522520" },
                { "ka", "c1cc685b21af28c15716d286efe8b90909aa17e1c911b8e1d2311f5e4f2ece5f1fdcf734bd5c495a1fccbf67aee202da3e2e10b9843e926fc06345c7dfc7657d" },
                { "kab", "b719ec15bd6903c867805bf06fc1cc32f3cecec0b66373e71c428a4727734d7ead440e59fc050a6e550e0f7288f060fe26b678007a1213e75621bfe41e682d4a" },
                { "kk", "3c66926ee4fbb56a4162cdd6c9bdc8dad71c0f4a739074654317433bc2fdfabb3f4d596ca26b0ab8b6044941d6a6c0518bea7a498f4846e1d5233a224e1b68fb" },
                { "km", "17fde11e4629b6eef817c032a525cce6f4999ac1bc6ecc766c1f985a06dc1f05c140053ce9c75240401fc4e5ae2871e0266d8206b4638df8fef82cdc03bc33b1" },
                { "kn", "1431d885df2fed44a64a87411f13665aab7453f456702104e1b45ff53793ba9fbeaaa96f0aaf7c824069b858ef86e4ae94bbd64f8d0e38d7d8e96439ded873d4" },
                { "ko", "dbac5f481ae37af307e241371210552124316005809dccc63ae24d9efc1825aa15d6815ae705b355be6b44416ba8c527aa902ed00d1358979b26be164b55fb8c" },
                { "lij", "42c0fc7f74609edc77156384b178e4647fd40fdd774573dd5ad37d36c958dd31f4359baaf66f1870879909bda95fe41af8a5ab3b77dfb6b391091a938510dd24" },
                { "lt", "3f667b679d30ba62c962b2f768ff17c8669eefe99a74b48b8e80917f0d5a3cc3e210d6234a68155f5a9f094b3cc7f997d7f2c545b9ebf24231194188572d8b5d" },
                { "lv", "e59a0ebb8a6de3dffa21fb6903f1ee61b72464bd3ee68e4650be36a9d384dcb3fec0efee31c3165c61dc49f4ba03e62da93996a51a9eafb135c882c21a26441d" },
                { "mk", "fcffb0e311113fc9d5fee76327263d7ee20dc8d39472687508562c0060eedc488f37b2fc02908a9e1fe76c7b9d7f7d8f208813ccf0c006462bb677dd5dbf45bb" },
                { "mr", "622278ccaa2569b81cd39645e719be587a74b301fcc2aee3d38b2e0bc483fcff057f466f86350be7320b7e3baf2d46ee20593a6cd4071451721d16c1adfca827" },
                { "ms", "3012b8ae430d07486b1bcfb2825324b448a147bfe54abdcb5d8a157266daf0f2b2bab34154efe9d3853b7a0ccd08292e2e5a61ae4896a1b0a8fcc63ffba0914e" },
                { "my", "ffd232d282f111c446101a346cf3794c86903735082395f47ed56304e37a21c3f458b01fce3d97af4ec8a0f923e32bdb837b36b1a1d406e98ef5973f81e156ab" },
                { "nb-NO", "614e13f73171050e9f9c75d568e7a68da73f77377fccd372d29bcd35039951c86f3c27c0c3cdca5b7b7a816bdbdf700d06c44dd0ea68fa33cea9325ea2a182ae" },
                { "ne-NP", "b5256017e4eaf5e583fb8fdf915e2e4e6b0a7c86facbf49219aa5bb457c75fa467d95d04b8a0c1439b95afa547410777d2fe78905e8a147e82232ac0e11e2a03" },
                { "nl", "06ab4c7d1932a3d2f9e803694b0ea6055010530e721d76c8ca7336e2540176f71398b411ba4672513a6b4f298af1556663b5edb15da1a554160b6811a2494aa5" },
                { "nn-NO", "e84ea8f206521eab501406ef9bcd5a3f360a9782bb6ae0e636074b4a994c3cfbc1cc0ddf80ec42da21ac60b936f02830330353e37ce6819adac06aa4e2a0b125" },
                { "oc", "53dfbfb31230ba931a09581537826483a9b10d214d435c4966664af9c464e9a6f2222ec8b818a34d1c889f01da21ab37f6b6e5a250b38aa8af432d2b0d0d2931" },
                { "pa-IN", "eae5d477d40747641ab7269643c66e2ab9419fcc8abbc43c4264af41456385b2dea0111fe5ffc983e9baa2189b05b5c869831af2b0c9c3f84ba1e10310ab24a0" },
                { "pl", "5b25a15571dbd13ab2094b687455d5912c482d693541dc13d5bb7fbbfe57f9f1e3f864408d0b3c4c3da81406f05e139338874d23d205a1c8dfa8e1cfb6ff4138" },
                { "pt-BR", "038740c1d93ed4188bce2ab36f1860f56a4dbe9637fb35bd54032776bef550c22cb6cbd32eb1ffd80960b08a61e3a95e67f12646266776bdfc415012b831b47c" },
                { "pt-PT", "21b03923daf48e866dad759467c4a3050e942f41eb7773dd0877e0e9f193f73d8a7f7b9f2bc1d2f32ce91bb203dc50410916dccaece247c9dcb65192a9be83b1" },
                { "rm", "92902bb6a82c3128ce9e5186954dad6b40b2b96a324dc3b2e140a5174af2d2554076c5bcc25b826d7f7c11f9205b684004eaf886f5096ba11898eb5b19afc1b6" },
                { "ro", "08e8feae268b1b541a168b25f3717584d4a624f30df7f5dd1085389d58490ddc084b512439786f271420872b35fc15000bba1bc35d467c4a741215dc05bc04dd" },
                { "ru", "45410c54442016b9f99e00b0c7e5f047b801ab0f0d7980b3cd71e75871c1c40e2cce55d8ebfec818b4f7706958bb31261c9fbcd14f05509b76fc3366e026f535" },
                { "sat", "63531f06f366bff79afa1ddc0b2096e0557fde9d27de147af62dc4f68c84274c05197e73774f3e118eaafcc50ecb741d21f488cb4ff6c1e63190508fdde4f712" },
                { "sc", "40024421f02447176b16022e733d0c45c222e9c64e586cf5fe76dcec6910219eb88dac8247df7656732315cd16771e6c9ed418a22b38985a1271c7bf3dc34882" },
                { "sco", "4b215383a69c167ea1e335e009b076c99dcf7ab02104e49c59236df69ab47ec725970f7efb0d420509e92f585679cab93bc31e3ef0076524631cfff10fce0b52" },
                { "si", "ec14054f0596acd0f17444ff369ca846dbb6ec31bb138cac3cf29904e2ed91fecc451835fd342b440bf0a6806a77b2e05f4171ac65ecb216f75442ea4514b590" },
                { "sk", "eb38f8b6f3049ab48b8c19de1ab890bebfe00987d21ea9b6a80836eb6f659af5433db3b9095afd086e68d648ba104d1de84d128b9ca798e3a32bc2e3ee45fdf8" },
                { "skr", "9eba0b10c7985cdfc242a7ae877b74dde4362def010b458cc09de184f6d3d2cc336648e76521b9d6768192c052403a57cbdcde1da31ee354559e8f8d97bf9fdf" },
                { "sl", "89a0ba5851e8d45fee06ba37fbc9ab97445847599f2f3d69e65842bd0bf665dd904ec90f5e99e99c4442794d31d6077839c133d2caa60edc24f546aebf2abae4" },
                { "son", "1892404ee2d1d2eb0446de417d59e06636d51e6fcf1c50166bf751f686c7807e8d47de9f6d9d5cd292fe34ce62f5c9686f58c030ac45dd9cfedbc68f065b1222" },
                { "sq", "9a705c9022a29513a729725ab81830c9e4ead9485624de2004c856c728bc7b0a61445bb9b645b908968d9e67321ff6b764e7f3067e1818143141f86cd7da6b8c" },
                { "sr", "c429c877643c8ef6db7b8e8bae42f1ef21e7f0ba250157a6b0bbdedb55231d89f2d7713a645a9c3c5c77cebc40a696ca444aa64995b342ddcea89f5ad4bfc362" },
                { "sv-SE", "a56a396b5b3aad383160eaeff4ede8a5da9a30d62c0acace385c021155d66b6b9c120df64d7d7fc98ebae19f4b1e06aa3332fee50339f200afbdc53d1780d213" },
                { "szl", "ab1f8d0298ab0452899a5ebfe4b153e6d5c05939312466e80dfab9e8ef078a853270d1e6b06952d36bc3540f33ec1d604a5d4a1f7c49a00d25820e2d7700f443" },
                { "ta", "6f8257466bc2a98301c2c4844f5b9c06e9a01668bac9060c7788382e43de0442d574daa8930424320a6e5f8a4982ce87f551db1601d7ebd9f68f38cf90175768" },
                { "te", "a58305393d243713fbe11295a6d6167f096576c98ad7d35cfe596d012a73f79956b5263c8e6b521619ce70107cf9c6c11c26e60bfef3ab4f21dafd3c5ee16884" },
                { "tg", "d2105f59423e4189ba6bad74916b6d7c5e85571f2f7db5d53859758179b89da3fd93aff8d913448799d12895df2a245bb11bd07b8c08388bdb2e97588e821da5" },
                { "th", "c4cf4f6df9cbee426e8b62dc1b9b476209666b33514c36e2294f5857c2845ec6941e2d74b44fb302927b71dd9f3a24bd9c2502f3253606403b9f366393a0cff0" },
                { "tl", "e25f244f564c4dae23e13d4531abec614d0f9c07618ecc653cc805b2b2b2ba04736377832c3c77725b6029b27856ba25b0d2b5b7050f326e691cd83057029eb6" },
                { "tr", "4e610a03cfc145cfcba82c6d4538440d0d9dcd2e6ac3616a90bacab61f00de2d9fa9cab0c449d54a146a09c94d39d726d321d6b596005bbd40a4bf12f90a2273" },
                { "trs", "5dd458a1cfdf7d763ff92ec210d7f3b852c536e6bd6dad1782ab9c6536766bfcabccd6c8bbe61ebb6ff5ae70de050da1884d2362febd77a6e3388ec6c70bbfd5" },
                { "uk", "6ae2106831cb128c62d0ad7d63a6f68d02eabac6e613ddc4d72558dd2dd4ef4cb2fa085832b10214e5a01b13bfaeb4f7345fed207fbee9e161b02bbb23d1dbb1" },
                { "ur", "3608ca225903c16530db81eb6d9dda6a3321abf3adde6ec96a08db2ab278ce1b74d2a67b2730da5c9b3aca200d29d5d4956f1c609aec3010480b956cc17f791b" },
                { "uz", "688d24f56896caa4689f1f75add0b20c6d72ef6bfbb87df039c5f4d2e5c35eaa988cc66f579a2aaadedbbed0133a84004cb54c0db991962b0390f9a873e0a93d" },
                { "vi", "8acddf8ca986a2c6103288658ab1046b74506ead983240f9dfecdaf81825e56f64d9ebb94f79d60dccb6a310addff9ac4bcfa106d20c74972d6bec705d9e8bd8" },
                { "xh", "c67065704f22f0311dfb7ae6d56a3c22fef890e9262f844a0aae0ff992eb46e5643036f1b01845c77b6b674ae4a88b36fd97c5f14178be7eae4d3295821e537b" },
                { "zh-CN", "509258ec819a3e31aecb7777e588a76d2df1ebbe88de626c86711343aa9364bc623ad3ca590130d0ed6fe8d933584d393171713a5eb6a9de93929bf91daa795b" },
                { "zh-TW", "acdc951dfd7cefa931dbe932e180e35b0413166cad7f92ff3dfe8170a47bab3d51b0cab900cdbe2f259494b1b17f7607a32de0c4fca887b4581b6abd8b6a50e6" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/135.0b9/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "ec0814cf27b9ccf5cc190404b7d4ea313ce5bf8e6f0271cf653db8d07dac4f829f13a7b040b15ddfc7c65facad5200112cdbc3097355d9db8eb190a7f1ace936" },
                { "af", "9057bcbbc22c7cd704d8ea728e703003fd86e5325ff60b7c659f083383a0e7b96086c815a40ae6c7dd301b34fb1558ba90d64392a89488bd5c9e2923b52c9939" },
                { "an", "6ef2382dccc9a15362a0e40b86d1dafbab15da70847853fef9e38ccb875e94907d63212b1491ea024b3d0dfd66a414c5a6a2a74242b63b08a662fe163f2e2fe1" },
                { "ar", "85731546227865cb1324b7e26474cf8e8807f6a0b893220122ce257dd69b16672c7150d45ce374983f2aafb00429e00791089c18889785ba638f3387c829df5e" },
                { "ast", "6b0ca3a471f9917f453b5e2ae488184af0f49218cabf928c077cf81c812d7c91ded8c3ac7453ed5d25314406a793b1f7d18b520f46c738ef7856443a8b7e93ba" },
                { "az", "48d99a77b6ac044e8b1173689b9bf5d18b247076ecd74587b1d92964993dd7a6dc036d05c7495b93708accdd5e014998ff55e0b643c374f65d55ad1b5aadd218" },
                { "be", "7abf3b6324c5ada28c8671f47f724b31d62a194c4d6ce4828df26945a3554c0a0259d82d3dff57203fd9a42a53d0b57353ec92a6f69f9139a96cff708c8f1603" },
                { "bg", "3627b214a40d56309d4da472e53797e4bbc3e5d3fb1628271fbc5c3390417e51fc5b3f4eb67338590a95f712633eefeddc3d69bb632033a8848bf3224bc58656" },
                { "bn", "ed6bbffaa62a439e737945483af28d06f69fefb7d5b33cbbc80ccfee8fbc55c8a7862d82ec41cda156b09cd1e2d5ff1af56e46bf756b23c7337a481b365e78e0" },
                { "br", "6309fa5fee85ce5045abe465f96be95a09fb3df30627046dea37863f2043ca859fc846241e35a1be8b020549298f9cc4df3a2d63259d8f878738204aa6271b8e" },
                { "bs", "33035b1fc3347fec7b724312e4217c190e6b31c9b20ffdae5b391596d5e7b4240843e391ec092432ede2f3c3d2b7556ad296ac0239d33dd0be1a23b6110ecfa2" },
                { "ca", "06b8caf4a3cf40adb927bb543992a8c72ca40c8fae2d8a522ebaa473295e17369bc9c9790cd34d6c27763e25277ac5bcc6597c8b2d2cf0be93bcc1b824be06ce" },
                { "cak", "e84b0305884d4f1c826facd27a4f21d259dcf89d74033071944b6b05109f1d35fcc5718eb44f8a46239a1eafdb6162099a41978bd3be151b0fd72ccc61fdf210" },
                { "cs", "c6beb145397f8ebc1ecf6c6fa472276033b582b701ffd165a16556d4d3fa2c9af853cc6bb28e129c73c143d7090bdef6e74be7b26591a4d3a2e79c37f9d07350" },
                { "cy", "03a29790b23ff17b7117a79a161d5ff52ceeb81770e44c0f26b454dfb2ca0abba83f484d8aa38ffe319d82dd1e04dd3ea6e5df7e51e8edfbbf12b69f7b38ec5d" },
                { "da", "3f0976742c128f040d14f071b2af4c6890fc4e579b0af5abb9d1bf2ccfd604a13b11a3a89147e36c5ddc6d6c91a66aad4bac5e344f6b1f0150bc82a1083d0d08" },
                { "de", "018a089be9b1bf90a3cfa84a98badf410b822154f8e611c946e8e283d38d4301dbd5fad2030da96836df5d2f0cfe58f30bf4b691e9c72951670dca92c2821972" },
                { "dsb", "05b47b8fd254c2ef4c8dc1ab024bcb41a08e012f33c808728d3d8ff23aca47f30331579035e735eece0c2fd1a247348c48f2988acb893326089f88724a235af4" },
                { "el", "b8378a452ecbe88c55d5752f41b28e4f779a547c5c9b359514f23dbfa6967783857d23c099f7491c7e78c3263b4e5b5a1929578395fff93e211519fc63141b38" },
                { "en-CA", "117a1d66c3d2fefe76efae20dd526e24e336a44d76f577d1cc0db97379fded8a17796ec7c6e2becd1c9492cf4f14481948ff50ae43ee87e5bdf5afc9d8a4b9dc" },
                { "en-GB", "e14733ed7e3c9030edf1c59fb701eb5ab9c1b4d05a8fb685259099d67d806c967e71ad1c6070f6481cbd2be6d1a65bf44d5aa08a69d3ada1536717954c48b061" },
                { "en-US", "96ccd6fcff8e9b32ecf2934abb0703fbeeb1210557acd84ccc6902cc0ccb8362092973e3a4152a098335ba38f2988260f12108091739f912c2b1b4c0a3138ea3" },
                { "eo", "5d5ebf6d406eb40bbef0342a0687f2976167e152a0bf7a33c54b6f83304b9036ec731ffd00b00b3eb27863d600614efb1573c57e5fd3a236105f8437ca1c1dca" },
                { "es-AR", "1ac5c14cdc08077c1b9784f619b73c5287f01ad6b40d0519b2ece6be7cdcb8529fdd4417db912f8bd2e941f19b4b2e055fa027ad13d7814d3af4e88f44c674d1" },
                { "es-CL", "4e76303216328201ab30f60f9a095bb078ba5701f8bc0d9f3079aaadee0d015e58c3d0206cbdaeff9ca392cf0fc845331097986be6d64f3d5e8db35a5b75c705" },
                { "es-ES", "299c0b4172d678d9a4e3ba35f67495d5b96a3dd8eea59b20e2e5e0ed5ec7423dba835f3cd1b684747b488d2fd0a773aa3a68b1e2a1a30f947469435d174c8047" },
                { "es-MX", "6b6871c85a0370f095ce7f8da25620f92034547382a54d3d719eadd553b08a77385ae75143d5179c4161368fa66140a9fd2b241bdd8de9cd381b718cb1ee7334" },
                { "et", "13a0dbec6b6e520177a0c77fdcb0832cfd15070b14d0f24690ed13fc2e6d6f32bf21403966b31384923a37564ed08e6ac0ae0601807be84c8da9709b0323906c" },
                { "eu", "2e918882e0e79c6f5c65c1abbc68485b252ba75e08608bad3bf7ba902cbbaf473d4a166b955aeb6017ca936232ae5fa5019b247462fdf49ec3dfda3f6761e708" },
                { "fa", "96b872c944daae6bf32717b3d08bcb4bd041cc361f639756315099c231ebdaa49d59a93793193fcffa084fb36bc184c20e95d04086f82e05e60be3ce5d970aca" },
                { "ff", "ce0cd649868f5e65476330e7639a7510b5cf91dc08176b13a31d49b2fdce492b6eeed87352a2b39060c070c0fcf912e2b4b660a42ef5404ebe21e4e6a579c615" },
                { "fi", "0dd1059634e165363154264e48ba2c1ade2ef9bcaf7c9ff70531b60946aa299b9824dd86f081b2cede5f645443127b5e7418f169bf12158818f155a51dce6f2b" },
                { "fr", "d87113420f242f8da8b505001f01a9b4879117d457ff660b220df6fe62372dfc647a205dc769fdf95bfbb57dee1a86408a27852cbc21c346773b3445b7bf3339" },
                { "fur", "97922a087e45865c1827ba62f0d4eb016397051138f2071e9b742ec53f56d9fbbf8df6da0fd5f26788d489cccab7c268efa5c60f164306cbde24370302cb0631" },
                { "fy-NL", "58bcf3aaaf9b48741f89b5d47b560b9c972b2eddbe91c50b81b368b6b8adc94f2e07adfba381e9a982da4e3a7606991e285c8b69219d846d7c89021e8b17f07d" },
                { "ga-IE", "f84eefb7a52a9170c38d6dbd550735a90ac4fcd65d88c056a07e1442ca4186c95b75f8b544d4d96beb3996b412ae3bd4c1a6ce98f81ea4fb481e88edab08df11" },
                { "gd", "54df530021b8a5f6568e37a388787a92ad49120998f349f09abebb4a0d5688eb9a9cdc8b2bd6c6e3ec8bee872207ca378ade9cd987324e45577d43dd4b6bb3a2" },
                { "gl", "9821574d01fc53b0aa3efe0e345bd3638cbb38673ec19021d6f44adce5e61d8a040d8c81591697c7baddab26e03bddf147e91974faf60ff6f7309a5fb3a24d36" },
                { "gn", "23df48acbe02bb9740071056bcb5815ba78727294e08f6a23aa4a11a67d6b20ca7dccaf54b1295905f5c25c5fdcd6502f05d8ccc50ab1ecf841e6da16cc8eeae" },
                { "gu-IN", "dce9d12bcc325f044ab502059fffbec2c956f0e8d0830a283a33a7612dcd2feae595e8e9e1772cd5d3211dcc47b45e8af50fbabfd8994d0516745dcd57777842" },
                { "he", "17e3b4f2693160390fc1ae6eebafe835db634cb8485f0fd7fc3949c1d708f2259ad918efca704f0e5911d4e73b6d700348f014f5a2d8d1f3beb44248d405e18c" },
                { "hi-IN", "43e4edab14f7190ceb9721944c1c86d547644451e98b930152bb74df35609ef1af171889be2bfd3d24a156744455472997ac8b2fe1fecb8b25548fde8d467e6e" },
                { "hr", "c21152e161ba0b7e01ea597d500f2da1536998ec6a8e19a6026771792fffabc04e21983d3f22137edf3555de20a839da2295262bf765593d7320105a0a12721a" },
                { "hsb", "fbf882403f2ed1a69e2d8be7add9ae2908e5715f06e66bdd86f0e68d5f01333226af04561b6b34f02fbb745fa746b05862f0db8107e206d5f4c30e9f31c4f719" },
                { "hu", "b54b7ba165bf98c033a48789c1e19d62ca69eb5464d7e6682a423b982b82bc6080f95c2e629997219696a893004c7d554e51585690b74139d8ced5adf5ef79a5" },
                { "hy-AM", "f068b99c15e6e7e4a418b0e06a9a2e4afe3b71272c7b4606892e704a67bc33727e5c4b518d6f1dcf6c27b3ed0541d0b202e18def26fd6a3f9ec0db1e1418a559" },
                { "ia", "9fefde3a1aec8c62b2911f6a240ab265802e8d88db2d70456508dff8ea1105018098b2a401b83e45c023b948ff019de9c8512106f9aae4cb28ed563603d3de02" },
                { "id", "81b37ccfbaeeca8930701a0347a5459d364e1b6e686541ebffe0908d4833c5787c7793c1a712a47d19bf1c11c3a33529d1ca5b31629c6e45ce9305fab7690df7" },
                { "is", "20bff1e999f4f1705da512afc2862a06e0773900c9800f3f92e866da0260ac27e8ff7c4da785cefb21adf9451de34ee7f558491ac752d39d1b76a772f001bb02" },
                { "it", "ef287383b397dfdeb242e6acfdc37cb8083cc5c88fc9c99d1aaeb4132df342af4400fd1776c1dc4baaf60753dee44495952f18b54ace7be4b5a286a3155d1c41" },
                { "ja", "54da72602a4d2d401a9b985981e83e7c651b191834aa383c8e60c60d468edb19524418db9b48ffe5bcd51fc8c6ab3c503a12ccae11dc657e73e1ccf818266ada" },
                { "ka", "a8edf84a181c07e7ae150749eb5d19eb8fcf292e78766cc523f17a665357c6e57d7f85a2dd14db6f6b29f089d808b7648ff0d3d0eb469c945d67e7cb4f4f2be7" },
                { "kab", "16b54413daf51ee6ce4f3f584129295400fd81f06e7ea2d535820b01c94529d85e120d890470e36d9581e69b55cd6f16c00c3f4274522f9ab5110be5788f2147" },
                { "kk", "c858165e6b6fea5689f25d89ca79b29bbd1ebb4b1f9d3d7af70175bd40870f74195f379631ed3c11ef28f68df73be8802e4111c175d1ca575b90f517a9efeb89" },
                { "km", "1a387e686b37f4c0269d16590242cb0d598df88a8a01a7f309b97195700e7b9b0ca0ce9c8776f1de4db2b5af3d5488506362ac814ae836348b77c4c97d8bef47" },
                { "kn", "e0db5e060939cf9d4becd3ea56e4a7361e8450adaeab79c1826a22ec31f728702434074c1344ad7056a8955c4fe786d6236691b7838b8505a0ea64ac35d0a9a2" },
                { "ko", "76c10d1c3a6b3d0b447eeee687c3fbd32e14a3d3e35e3cdf91763993cf7645ac0f799c479e13ce73f3af8ac2f26401d2aecb8fc8038cebbc142784bdda05fb9a" },
                { "lij", "8f4454cfaef7764fb2ceb0b3566dc930d34f95eaebccba27b72343f3e70b9ff038379260a10e9b802129f9cacf23966d4bae0b384204be85e17a545c750d08e4" },
                { "lt", "9b397dfa09190f02b96da40d8e4f7acd36b8794e797a15e416dbffce0c7ce270680a403f5844fb6b44552bc5f192a2986ed146d25322a285114104e63e32207f" },
                { "lv", "eac9bcecd710a9253ced3ac7535ae90a1cbbdf905fa5351f796e950956871b7255e1a2acb04d247aae3dbea0cc311d5536d7da740d70e6e0b0cf4e35a936b26f" },
                { "mk", "fe06c44187ee2b66d47708b57f8c61e185a1626f061bfd7dff580afe47d7ef8ec3316e646c7c1f8e968503c1cd75a9e35b584ce2653898c6bdd0f632d5b77167" },
                { "mr", "28f586644a146563922673b157f8458291d8cfc8c4c5c61685dce34c6e086be1c7d610796b00aa7c0a57cf46935db7937fd72796440e4be93366454a50511659" },
                { "ms", "4c3a0ea8d10b148e02dfb3f7f3c58bbda2bad053f5ca9fb57db1b2796b0932bc31205884fbff053343e6b39b3c534ce945b971a0907b37f53018967345c868d8" },
                { "my", "dd68a182374b4a27402e6f54ed450ef4a7fde06729fc5f6ddc27e75ad467bb5a508a556df780d5167c931c9f87c1fde074152945a0b8e24303e2f7b9a247cafb" },
                { "nb-NO", "eaf746620f3d0bbd68457819ee3cc30f655cdaf606b5ef01760f779d6a2142c6560509c1da4b4dbe264687eaf76a586a96a99b3b6a12024d0aea8bc1dfd76dfc" },
                { "ne-NP", "6f6e91af950850ff38b75fae91351431050586231254070bfa3ce2092da12267f81e7d02d015e3882637d0c42b85f01f26320f25c1fa2036845c15afe13c856d" },
                { "nl", "8a8a9e5fd8c3ecd93dc28f7f9b77e75c4d0e417a67d2605e43aca545fa4435ef84b55dab6edebe22f71f98aec240e0d70f7e21add85b6ddbb93d07a75e1b3395" },
                { "nn-NO", "63bb15bb3cbcfaec35986aed60f1a216c55c24e98c5a58ec779028796e344fabda9bd673a2fb802550dc91e2e21a5782e590b8f1515dfca0842e5620ed8685cd" },
                { "oc", "dc5c95eb42f8b0fe1a168de9f53cae5104d794055a51ce4e20b4da0ff042eaa661195bf763b62027b47fafd6c022e89df7e490331f3b382d61775085de4cf38e" },
                { "pa-IN", "5893f4424e3ba717765d6ffd10b80f0202499e3e19f2b4bd93092eb599ab184c0d135d6efefe8d6f0f0667f82dbda4722a50c9502ee12b7a861fe3b876d77985" },
                { "pl", "c056da5e1fa8ba0909ddeae4d00447c10f102cebdcb814eb6e2f2d28f9110ef20d410477159c32ee5e2a955d402cc98904d8471b2c2c1f6f20c78692fa2e4e06" },
                { "pt-BR", "847147b9bf536198f38e40b7b9067acf67679542e71f06ee2b046b4dd352124a33de3109eea46d5ed064fe3cc536c723eba6196b9e690cb4ca7e1b4170d3aa4e" },
                { "pt-PT", "d9964b72ee10b36b1f21d720ead6cd12ca93ef0cc527056e7ebf37c57e854a1b3e70161300e052f823a50be41b317913c9cb5aed46ee7714de425d3046903b4f" },
                { "rm", "f2c73a11e26af386a6ec7d53389d7162bd76f253b0dc0fb37e107b2b6028356dac13f2fa0299005306cb2c31b3584ea1dd5011d503510b08c908b1572dcd4c77" },
                { "ro", "591c29763214b18a526e3bc58ffd0c07ed1e0a49339ec8d3eec98e4c7901bb05be1f52d9bf829bed78023bcef0965a86b52186ca24b2e03e4057f0d20ef9c3be" },
                { "ru", "de21e6c6d09c8d74c08b2d83599fc537e1579e7a026c8fc26b3fdcde2380808c806d78dcc883483056b978254c5cd63a5a96d0efc9289e79aabe519cd47a4cbd" },
                { "sat", "d41dbef5a8686088cb6e2dfda0e4b02197d151d058d22531430ce50634e3279e345f826c8b541653df41ca34ca599766ca5aac7646527602f4a5ec75a8812d4c" },
                { "sc", "019b17fb2e15d3de6fa72a5b98dcdb531e7ae1b25934998e71fd0598c659cf4455477e10fb53f5606b23761137e2548a5089067e8bd2189bad10afbe30e387be" },
                { "sco", "f96f99b4a689e79ede3ba0e23b55a593f6fa6e3fb940e232d840dd1a638a718292c79b9babbe543ebc51d060256495962c47152cab846805ece0c701239ab0fb" },
                { "si", "0e3d7ae95832f11f154e922b5ed6e9d322cb2949741f39e8592e088e35864573e05d695a7dc7f1b01c25e61587b444947e078e252ae4fab9797341635b535e54" },
                { "sk", "cecf344a1f851ae568f242645fd1df5961f4e8cd50d89219c8a44cba9031d9a63a3512f9259404c9da5fd0f7aff53b42f2abdd7bc481b9c4afcd0a55a270a47d" },
                { "skr", "78fef156374675cffcfb6fee10c22053a9525933032c8156b3d89cd41a3fc055a50c13435763a1dfa607eea529be4e9a818ca200f8b0bd67e08b1462b36f45ed" },
                { "sl", "13b314d4e589bf873f559b87bee7a0fef14c6f0f62cd2a47b529887e1e81f9dbdb33cea58643a5bb2b45933e769a804795b8caa608f3d9ed874c53b840f928bf" },
                { "son", "256398cf8198e4c39d1230a651230d40080036f054ac7cdc9381178487ab6cd77530aba0e0b748f74fcaf47bba7a4734f47f7ff436fc94dafc28c2aee1d2a6cf" },
                { "sq", "b241df76c0a98ce86c51d1062fd4ffce562639467e4e7d81f19a96b2d07618b99a950eb5b19458380f9707ebd33b322ae02a445799dd31e97420cfe516c800a1" },
                { "sr", "0eb30811c45149a37e0471cf9239b12d352defad91d29fb502058e1df2b7b7d74180843cd86438e84b11b02df4370c8b1041171b5a304b86eb0ab4357532459a" },
                { "sv-SE", "4e13edb14be665c24546efad1cbcfcaa6b9ad633f528c1e55c4073b2a80c952b2cffa1c72f32e777415f3cd76a36bcc3827c6ce466b382a59f152d57bf3737dc" },
                { "szl", "1ab94aece598b425650403ac9b75e9151a411ad7a93214fe510422c3981cae0932fe4c067e46656e9b811bd17e967a068c9ce70e910d324a316b8bf023a2ed59" },
                { "ta", "a08e5acea7adcf7ad15d847705f05cc13928d67cd4237cb1c211ca80ce6d537c0061b06b7b1b35f37b4679f40f3b385d8d8de13acfd1f28efa7028ee244b19ac" },
                { "te", "718077c367b324123e150a68e48372d22895a04734ee0b8cd3a99a64ea275a07a99a062f9f6db7ce3850ac2b254c429bd597d4684935e0598a9424f45e19a3f7" },
                { "tg", "b6521f6964c294c0944e10621f9f1d6b212eb1c16a08fa36ba978f7280a6cf0f12f888774a4fa29b428c91229fa8c88844be5b54c60e87ade3d415d9f9f6bf24" },
                { "th", "df2834c9141945b03650a09a143f0082b1efa5a3208a8ab4d65fe34872e3098046fea1006abe6e3f809e1006be73de0206176fb260b1d047cbbeb0e5473f1b9a" },
                { "tl", "34b0f2f1628249d8470bfd58e64a060be3d62bd70cb0ad5b47fd19c846eb2ed00b09b2f32926226fcc5f064fd85f69d1be887777797cfdafbeaf11727d4e1edf" },
                { "tr", "8651d74c8fe5e8e1f792c66c5526ebafd0f0dab2f2475a258561f27d5e5e6f8e0fe9d9c75b7dec401331f41da1d840fc89c689ee43f01385ce173a532ae19529" },
                { "trs", "919d9d1dac40947d9bea8ea11d2b6cf9aa1e78afb5332c368e4488287ad7b308507aba203b80489d18f0c1b4d6da65987aaef175c50523122eb545d2ad855f3f" },
                { "uk", "7834493e250d6b94dd22abe51bb6010dbc51f956b14b02df1a38683008e0cfa3754ca4fcaa2edcf456498356b21f1ce29bbab8d07e3deef8f5df1f57b698bd4c" },
                { "ur", "1a695b70f34a6b0e3a414c86e9418420e719805e805041a3378ec1a848e911b23f39172ff995a2697ff124b6e53c69cb5e69db08c6f1c93387bb69c6b33d73dd" },
                { "uz", "e9936a1f724596ef6a223edcccffe4d591103a9d3eb2a5b3d47a8937e0365b528c5eb29769f7805d7211b12669928fcb4541f0fa11bce1b25e9d795d59d2ae46" },
                { "vi", "2c8e8fb6e3ffd93d79826e2368c9d94adcbee46f37b8f9731c32740dea97a739aacd7a6bd2d4996718064ff32614d50b69ad9f23cafd1d8f63d7388bf9cce019" },
                { "xh", "e4db084d7dc2bdda76fc191e085dd4cc04f9bc97ea7579aa0fd2534dbdef799a3dd8353c8c0da046483beeb891ce13a8b574540bffcf1bc40edb40de898cec46" },
                { "zh-CN", "976406ba952f6134383e441174c0cc9e8c49ac284280a923f63d05b063a076b380fd242b2307f80a50e63f8d186f0e89790b3e780b0b6d4c54f2b7744ea1ebce" },
                { "zh-TW", "349afb27c15a081f8276a63748130796eaebe2b14e1a1c45e86946437baf895a0f299f8dee5c6e6937f3dddcfed083309d730d634ff51712dfa3c112a65deb88" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            return knownChecksums32Bit().Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma")
                );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return ["firefox-aurora", "firefox-aurora-" + languageCode.ToLower()];
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public static string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                htmlContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                return null;
            }

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            var versions = new List<QuartetAurora>();
            var regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
            MatchCollection matches = regEx.Matches(htmlContent);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    versions.Add(new QuartetAurora(match.Groups[1].Value));
                }
            } // foreach
            versions.Sort();
            if (versions.Count > 0)
            {
                return versions[^1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/devedition/releases/60.0b9/SHA512SUMS
             * Common lines look like
             * "7d2caf5e18....2aa76f2  win64/en-GB/Firefox Setup 60.0b9.exe"
             */

            logger.Debug("Determining newest checksums of Firefox Developer Edition (" + languageCode + ")...");
            string sha512SumsContent;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                var client = HttpClientProvider.Provide();
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
                    if (newerVersion == currentVersion)
                    {
                        checksumsText = sha512SumsContent;
                    }
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer"
                        + " version of Firefox Developer Edition (" + languageCode + "): " + ex.Message);
                    return null;
                }
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null
                    && cs32.TryGetValue(languageCode, out string hash32)
                    && cs64.TryGetValue(languageCode, out string hash64))
                {
                    return [hash32, hash64];
                }
            }
            var sums = new List<string>(2);
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                var reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value[..128]);
            } // foreach
            // return list as array
            return [.. sums];
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private static void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32-bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = [];
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64-bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = [];
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value[..128]);
                    }
                }
            }
        }


        /// <summary>
        /// Determines whether the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Info("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // fallback to known information
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return [];
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32-bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64-bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
