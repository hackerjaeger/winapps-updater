﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
        private const string currentVersion = "129.0b2";

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
            // https://ftp.mozilla.org/pub/devedition/releases/129.0b2/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "23271afa6dd5021b46d3b5508618ca09bd8a811c617593a0b5375b950d4b5588db754546cb777cad45cda30e1d0bde06e11da66e51679234c201991b26c34b75" },
                { "af", "7e224f3930dc44a1649f15f9b284a58099a413b0dfd9fb41a87b2ae70885cb0704b84143291dff92892e1efd94f634bfc183dcb137cbfcee0ea90b934383d2f5" },
                { "an", "3cb7000f6a7b3ddd92cc91674c49645d72551f0cd0b9f66c4bcdba22b55385601be3a2eed32e6d08770ea948d277db785c965f448cc1ecb7461f516d119b09db" },
                { "ar", "3969cfbc9abbbd24592c5f23aa84eed6281c284865c6abc0bad6c477156882a89dd4aaa48cdc34d8a709bd09d7af7037f36a5c36d46c68625fb1a2a53265f981" },
                { "ast", "83c5685ce86688385b6c9f65950bb2a582f2c0cea367b11d36f2ca5bdd6cec7f956dfcc8b0fd5c3ed913d30d1fd9045f3f0eaea8bb429809e0401deaf19734a0" },
                { "az", "8d322cad129409a041912fca35e27455968d282d48e518c5741915e6a61dec833d85513cea69424f893ec790c87fd9025206448687129212547e77f18f327d7d" },
                { "be", "024ef89edb84b456a8c4792043b3f19193c967d01e2c5be8220dc94b3a2aa6a4b04e5c5edab85c0fa82ed7cc95d388b11ecf7695a1c1fdef0a3f123f2a27fabb" },
                { "bg", "80b38ce695bd2056d0220d7b688a89eb08c0ed42c8fab5d887ecd749666229b2f359517979430eb9f0b7e2e70fa07f2661d02fd54811a9692c22761a085358cf" },
                { "bn", "ce8861e3f45b8dad5c44314092516a27ad181a722f5a0fc5c25d56e8b7f1d9d38ab07cbc48d4d5ce7303ab999d5f93319d56813db3a4033b554b0ef746a8f7bd" },
                { "br", "3135f2ba2294b1420d2091146f23127e535282f56ade41b654fa3d6572dfcb2145518bb0923ddd96ad60279dc18a47fa4822d7e133c6e221c769aefa94acbbf2" },
                { "bs", "99b39cf18bb2944ee52e8232d565698f956b848bf3c3f3eb9ba9cd8be4a855dbdd55aa02ddf5339475b9afbbb93619886171f30cf1a6e9acc1f95384f551cb11" },
                { "ca", "00ef02ca3b325e0554fb927cfd0ea739047468ee97ad4109dcf241833c0b27f14317e0225d102f8f435942ad43b94cc68e10b6ba6d2aaf942d01c3fbd100a85c" },
                { "cak", "42066c55aedf473e13d6dc78da00c1d1ee4c5b855e46c70c682a8bf7d1b1363f454501c8ce1f91757a184bdb8bf2e163fe87f357d840eebadb9088867bb0c153" },
                { "cs", "7ad2ef44af67f6c34611f6165f9f85d3c87e3fa9c07ae01c3bd844b053d013b6e720fdf725bc9fb440ecb04a7a3d594a91b88f290e651492cfe4151bfb11d09f" },
                { "cy", "2525b9a6974a6d23ee5c822d453e3f0a7d12a09afa90e9780193df999e7dd8ed36c241a1cea9517ce27c8edaee8b6c689ec6e5094ee3805fb0317d7643233dcb" },
                { "da", "8ad784d26a397ee9c52f38649c1df4bf97e875529c33fa449626ee28f5c1b67d5d2df49d895c9058fbb763695cca81b7bf5e43d798a57817c241c5b88cb6357d" },
                { "de", "e12ef8898a90089fbe101f377ea80014127ddc35bec04e3334a1b0585e9114edb311066e1a3afede95302a934e26b9c8618fc5d77f65e43700703c434b569730" },
                { "dsb", "a0e2290383de8dbe6b6c77ffddf17d80610f7ac18607d09f5513485465db3db6130fd7bc12abe7340fcfbcdf148d786496d4ed1ed88e92da373dccba32f6476a" },
                { "el", "2610605991a74fa03d659ed9455dd07c23455a90ce3fc61a0567eecd5cbd4ab96d665b2fb86432e7de7b32faf71a577067deac8e8810186c81867830552160e2" },
                { "en-CA", "cd065d4b3b08d6d86beef4dd006b20921508156f8f32a54b179755de7e55d66378faf7dbde53b3bcac4dda83dab25d3fcf09d834c897bc9f433db67d1c919300" },
                { "en-GB", "dcefed448087de71d79d4a4801b30889bb05db8e0ec9c0d0cec87d8007fe1ac4091287715b9001dbb4bae53c28a5a9df4ab59ad1f4a20d71e540ac21caf8091d" },
                { "en-US", "66f12d89c9700c3bef44047e26944e6457c163ea0a7deebc958ee76b40d86bc9aae3deabfc9029d7bec956dabd4877c85b04c9edd75af3967324d4dc6a133539" },
                { "eo", "d3a4490bfcbb02d431f3d0af7486c8e55a595d2aafa5c47b0c650c04c4aa2f5d09a590c2700ab39d829d484347f2f30aedd716ec8a34bca44f1a47d471761ac1" },
                { "es-AR", "45ea1d487ca32a2918c4bd5b3a2005adeb67d2452f65b7ceda32cbf0d146f522ddbc29eae8c16d2395c227c5d108e501bc51e9324c8902b7344cd6f4d4496f86" },
                { "es-CL", "ee207412818c5256a992f2a3379ee4effa64118e4596a9431bbbb39a4af507c4b2bba3fbd953c52060f058463dabbe4be085b8b98c925356722f0557711de119" },
                { "es-ES", "21728f97fc9c6cc6867773346bd957a3097d552355e2291bdcecf69dcd6661eb99a98256dd0d30727fd490febec5b999efa4b27e2d1100105434c6cfc792c773" },
                { "es-MX", "96571d28ec8ab32576e01484bea9a237176b66191f81ba4203e6c111f4b5d04e611f2a742e8962791c39d4c4c6abca517aff120502771721c32643a2e340a82b" },
                { "et", "16e76e56a503acb712e906469e7c1816da5112a53855ada2035c89744689528044aff987be38cf2931e107b36940d1beac620417eb90d00235039b55f2ce9309" },
                { "eu", "627d35b83680809c8f90369225f45f8a5bddaeb02e2f6a51f1d15a13271742e574c069708f9bf57fe8ed4b03b1462cee4759c4b7f6ab3d87a5b8510933d79486" },
                { "fa", "48bffbcc5bb3bb8374156e05e2d99351e23c647cf3cba1220341b638d0013364b474cdb20944bf86cdea1086ca3c147a2db77199ecb0cb8913bb1048a391ba73" },
                { "ff", "25c38cd2257d26b3320831633f0d47ae9a2c5080e22aa57219f4fc338e9d7c209410e11bd7228d04189b6c727b1018d1234db1fb8adfe7d502e3938a992ecca5" },
                { "fi", "9f36558069a2ba26da6e0c530b93c6393834224e05f388175dfebb13485644d012bd69ad66020f10828f321f72419a31d4daa81b371f02cb5e8977b7f3e53d02" },
                { "fr", "e2b93d3c73dad45a44a9b5a892bbb308eedd5a01d6c677579bf02650bbe5a18b1030fd83777845bc8d797248a969c0f90db133d2984790d72c2c6bc56f0d762d" },
                { "fur", "1da46394a6a30b9abaf392d8ad53e818fdf17315a324add3bb2ccd85f78053bb8a90fcdd1273109fb8dfcc96bcab55c2c4b36eeea06e32a7fb5d7c16e9767404" },
                { "fy-NL", "db0a18f3903dffde4c832cffc9e1cf47198e4c0e4ec61d31387f30970ac4b8dc9b4f906f13e26925504f008fc754f63bae956eb4dd47854fa4cb4143279f9433" },
                { "ga-IE", "fd2e307b0fdf23131a5c39ec35b2547ef8935c15fe55fc0a3e0b98fa6c40b2067acad85fdfd6172620ca60253f71a2ab372d05359df155829f02ccd0bd6dc33c" },
                { "gd", "eb07def90e0b1a3e884391d71416fb3bb3322dfb5f8e1fb0a779a993ae98bdcd61e358e37006342f639131de91748c220113645a81ff99fae2a897f6f35dd46d" },
                { "gl", "3b04ca988472c328ddf24658012575c95572fe2aaa78f0477a4d9e988510e3bdc16dfa7da283c8902f0fadf12414e85219bffde93b05f4bae0e25ff53b7bb280" },
                { "gn", "a975dad681e06c4bd81dfc34028898f68caff1dd47b790131a6415be3847d7df1b7ba505748bb9d69c3ad557f201eab121f6f40c4694aa0baa52373b62f4ef6b" },
                { "gu-IN", "b6c6fda5f9556b3f6f612fb23607bb2d6de585c66777aae242b76babc7eda4237b1d1588b0b4b59afa478931b475d1b941f919df637a43fa8809d7f34202b3a8" },
                { "he", "340b6d4cd29d9626f0469924de00a1bf7a9f6e391d058b7d11def432320493f61554585fb12a5ce91d081c49ae15303ced30b9e4d358429a3889681e2085f606" },
                { "hi-IN", "affdbce47e3815f64e24e9042d4f48e998b1b7ff71511f8a9bc68c9bbc044953dc8d2803de195b17a7efef9146317053769187ab601d9df84e1aa69095acf8b7" },
                { "hr", "ebc92102232d33813c58f50f49fe8e6c34d9f8d6fb2ab4d5db1e1e2002fee4cdff0f0c109dd39694b58ff3eadc6a1c117a8c5ac5fadc23df388a26a5283e3a6e" },
                { "hsb", "f07f23720583275a6908b7bbad5c5b0cab0628eaa6deba980d4652a6c825835119e0fdfd38effa9e67da18476f303dacfc702d3a2c61b9c5db1d30cf8557f625" },
                { "hu", "4ccfdcc2d7e33bab1063cf9398f4afa18442c49449343c73ed89be7e0161e6eaa847f2868097e8f3896624a04dfb2af264a7fd8bec65f31f1e3194be3b0a0778" },
                { "hy-AM", "082281c97ab1011eb02ee2a456fa91d7aff2934ed6bc691a802b37083e01668649707010f2679a8b9f9a39deddf50e7f9d821a43a5c70fb48fdc87a4a0d9766d" },
                { "ia", "770e981f19b46d5692f58be9f130511c44394e3618be913aa1ea11b9956473b0f506c174090a405f63ed140f935d2a67269306d68d613908e776a12fc8cb278e" },
                { "id", "6517a9330fd2a880be4018a356556d83221f653fd28489027d3d372d1328d4451742c11809ccab71a7b0cc6d69a4b0fd37de98655a8cf77437e6b28435d17e86" },
                { "is", "a8c2f5fa56ef131282d23fd1ba7aee63e82625dcd7c63f15bebd67fbea3107b1bc5f87034b088ee0780261aa5cfb0d1ade0391240c9a4d0de045af842d90fa5f" },
                { "it", "9d354c8cbb0404e6f52e00c6a5484b54bae2ada538f68b04c9fdff2a28c08e71f0df6b6e3344dcad2e8670a07a2cc9bcf68a495de8b11444bb03e234602d989d" },
                { "ja", "9e6a6c979086d310fa91d968b44ccece72b2150bb97b90f30e8d8d87d6d9bff78d0f75e7b40051b863b2606b16f042a8ab1f541a8815aa8537dd1375a28bb9ab" },
                { "ka", "812fde1fe32c7cea005e924033d8de7724799925301146219d2dbdaf8035b2f908250f65693dfc63168a6957beca5ee5107b74b03d57b58475c0e462844249cb" },
                { "kab", "bc9806569d88ac1d159b17e91c9d33619e19d41fa7a3331e0d1cc1f50084fe821d6dfe346dc724b8fbf209e232a49576951d211ff464d0566bf31483ff60b5f5" },
                { "kk", "49bd74cd6a4dc628fd36111a31f40972e65bd1b9f43196554dcfa114c94f319acad58346e0d3f395bba9141ad2c0ef464a3a4e00977705f1b2678d06bd7eb058" },
                { "km", "7819b512549e866c04de617dcfde44305d0acbe18d5eeec528a0f0f0b6ffea9d2f514bf06fdcf56da5273676178ddf8d0cacbbcdfd7645e59623ff17b133a529" },
                { "kn", "c356df0f5626062b8e19bd19e90d86f8bc77a7fd100a75cee62f3061f5661c6957ab52fe1d341cb7b1eca748e87989908298c48ae24ff3e5507bbbdcba31ac11" },
                { "ko", "8b7d3efc9d1e25f1c0297bba727c317ae57f0d8dcde6e800188ed022806a3cbac77dcad54fd252e793efea8e5d197b1899e03e488f983ff570182933019a9593" },
                { "lij", "f016e9c5156d1f91a0635ca50dbc5cc91880c6ab826b4b5f23546608a210caaa62e6111e9e93a03a8c7da505c1a08ff431207b13887a499ce7f8328ca9699ba1" },
                { "lt", "d2ef7e029f082004907267f17e745f9e3003cec100925479ec60f9e944e3e027fe0ab36c7119b3525c38a2a08bb7bf9a202b28752d0de7180a4482d2d0033e10" },
                { "lv", "7ea86e6ce6fd6dff6fa056f394de7bb294b2a87a0ed9ac68b3abd44ceffab02e920aee0f8026589f9eb851bcc7fb377c7589764265c31f02d8714c1ef2453b63" },
                { "mk", "64ee506c1940b4f9706b912e8cb2bab68feeb723af3918a296cf7fb30e3a5aea4c7bc54c7c5ddd9fe4117e05eb7fea2e487e82d9cc91cc16ff33a0d0fc666cf8" },
                { "mr", "16aa6ed50f38c710b93cb28bee5c7699332841d555b31dc03b5b56410d14e5eb39e8939c2aadddba1e0971def11f37d5030fec1ef8f6c23f1b25abb2f6c1a82b" },
                { "ms", "74c1adc016abbe1351bb9723b5cb423c50f8882242f32e85cbb4e743f4ee04c5652590c791c4f0ff55a99abcad2927d69f4f3a88f0379f398a9219b94da96ee6" },
                { "my", "b553d706f8c32c1a2b9a0e313861ffcf3b586fa56d39040e152d3c8fafd358b3228b2848136082d183b3fd1f0f58af16f3c1586b07ad0fa8e31621dbbd81c78f" },
                { "nb-NO", "536dd112304c014a7cb0a4dd73e47eb78e36306cf597c5fc27eef6032672745dde2359a655cb40ecc9a96a48895ba8636cce2bf849088e9ccae4181504a0df85" },
                { "ne-NP", "6900a002b0fc55349b7084c159d2bf813a9566bb01f1af8ab233dc004eea5e228cb49840214d8020aa118edee44e69d1959ecf1c2f3a850f401ddf946fcd8e20" },
                { "nl", "f866c7797e4c027cab0bca17768a709ad3effe8a248c553663e3ebf937afae2987d66c2e4ca2fe926dc285484f8023a54e98299dac04b769a916b2760ac42744" },
                { "nn-NO", "76ee09aff972578c9c7b9907997b46ae92dd44cff509e5234015534765f70811f9c994f90554db290aa5a8307854df197783184709b6ef28fcb305dfb45886db" },
                { "oc", "d4bcf575c232072fbe5456b9141f3fc66c887b74458b82c2f1abfbf441bea3ce9036e29a2fa195e014418152da9557731bd57569d534473035869af7b084e039" },
                { "pa-IN", "4d9c0eef004b973cb23f185134e7f1f4f8e1ae801b8284fc5a2ea9801a53aba0ff537107f5aaddc19e4a825e8a094f3508031080cd545db1c385b85748b4b7d0" },
                { "pl", "bf0040ded726590f4e42a509e15bbb6f23cbeb57a5adfa4fdf45f691be48f79410b9b791926506d279e8766a49c53d36331ba64031487b66bd6719aa6b386fac" },
                { "pt-BR", "bd4279b75d638f35fba70138619a4557bf0c62fcd8fbfafdef0ca2af353eae900df496b3b0c09436563a731b9c3bfee3d1f5e78481fade3efce2c67c48407dc6" },
                { "pt-PT", "eb13f45aa95a1d3cd102ee0bdf8f2b4a48f6209afdd0f72e7a2d993bd238972decb8a250f34ffb5673a3dd7707e5142560810cce8d5a56c3c3ebe31121e82c14" },
                { "rm", "1926bd5bbcb641f951e525399367b4c15c19568db36ef169693f841d55480a44a1d5663b97d3daf4eb3391972e96badc4466c6686a05dd003cd1af42cfb4269e" },
                { "ro", "c826a203bf11b19f37b38cbc9f6f25eb0cdf22c31cdff69411f2153e8c923fec5682eec2c974453a2e04efc9bd1d39f553a24380be8307e8fe0e9233ce8a6856" },
                { "ru", "42c9b4f42feffc0819f6a802465adc125848fce11e76b58568a33ffdb0d24bc99b5efe3254fb6e6d1b9f135d2f78efeb0bcce3e1e31ac80afddf5c9f6dbf7dbc" },
                { "sat", "2677f277a4d16e3e88e086c0ad97cb9cc54c1988b2005baa14e1a63d38a9301f6503fa096f8786dacac619a56ac2fde8186b046995819bfba5fa87144e362bac" },
                { "sc", "3369d29f41d00e019dbdd97c474791196b418e434c3f361a1b46f8079ccb79011c7414a45df439f13240e175cb5177eec4ad1994edd420a47d4b2c9d9e05177a" },
                { "sco", "40fd8f34bbe39d475f5b661a86ab974994a05ab8d7da203f9b520a388ddb30792f9ee0abebb9a2f92e9f3f41f3ae653dc6ff2d15edeeb41e724915b19d6b5088" },
                { "si", "fdd962efceebfa60173f539156c616376dd329ecca97e4a1092ca82a071d5b20cb90d30f760ac7b08d9c3f50395cb8ba7a0166ef0998674f44b44dd814540795" },
                { "sk", "15365c8b805fe16adbdf4357ecc21192855d57fb8473fe48a34c4383a6e8209ab1105b3d32e922c50f6e44def353595554b35fb3ffb8ce6d14427e843d857074" },
                { "skr", "e0550ef3f6dc09b02c6a36b3228fbf66a046cf108124bb9207de6797f86ff31dc7dbfe60e7a2677dccfec7c620372a6619284793c558b0eb7d77d6da614876e0" },
                { "sl", "f8592b15a8529497da3e844d7c3f3c7ee57ab69f1becd6e12a28d83dd7bcf3eb8d6f632df5d7798a4f0f04474faf5336068f760d1f4dbcffb697fe7a6d7e637f" },
                { "son", "82c0af5fdd7959d546b03b687f7343876c7e1e1c68f687c2054a1cdcb4d33d082da19dd6f51fedf6d2a2da86da5b51502638712217d061e3ca502aa72deeaa10" },
                { "sq", "649b6eb7d9a5de239cafa409237325d22d99c8b3272ce48ad98f7abdfc9601f0fc8706cf3038a8d13802ba175c25a42a8120dcef792eab2db345cc62e83624eb" },
                { "sr", "6fc85da0559b625d432778334207c53690d75f74885b97d4c0702db98d51b8680a53b4d717cbd0d602c9b2dc51c5e33a659127f7a7fcda5eb94fcfa66db7e72b" },
                { "sv-SE", "0515bbe9f64d4e8d9a41de6d03d4594441c477ea30ba9c8adc49b858183a23050291c71e6ea395c1f03cc1e211c194121c8dd1c1a792f47dd27d6dfef890a26d" },
                { "szl", "a3ecc447be935e09bbbd510f670d50f5afd7612413804007f66fb6e237f015efbe4a97363a353c9623f843f0def5cc8e0ecdb9f07f941800dc278acf8491c265" },
                { "ta", "baf060827b234e444f5dbdbe7c3d11900163cc448cf1c629d883f2be1c14127fa6b6559e32eaa0d8742f4f1a08274f2f83cbf852ecac07a8b5864cf70ce77ad6" },
                { "te", "86bb63370e900c69c44e1978cd072784ba47d7286abbced63702f54d418ecb8ca6e462b2f65935e049815a73f5fdc15b23db9491994fbb51ce3b3955ecff95ca" },
                { "tg", "b4cabcdcfb9b807bd790b04c58d97f32233eb1d0ed4031c44fc390b1ffe696bc1f7f9923f8c83d8d35803f063236ee3c2cf65f4a4881e0e4f1c8866a673513f4" },
                { "th", "fc38e0d62037c3540df790f4f02eaa87d7f012687c0ece7f84fbd32cea51a95a41d34146009028db4e109fc927ce67eb3141cde808bb941a5d363f9783c949e0" },
                { "tl", "0443bcf9639af9388b95c678788bb4960b33601571ded706fc28ddc74b0a97b3c0472b7a57fc39afedb74c1e05933d042fbb2af26dc38563a0072a95fd3999f8" },
                { "tr", "2c2376ae5dc2f5e46b47599c6ceae01f6ad483526bc3584729adc039183ba7d24725db66213b882fcfa926c6aa4ee3eae2ae118813950dc73b52f80d94d424ee" },
                { "trs", "1817dce9472347cb160cad2de7100596cfc9578abfca054e88916dce15d3b80b923238c7d024e823cd992e62fcc0323e9a1e2aabf95e5a386b3258b607239ea6" },
                { "uk", "335143599eb025777bfc092d5a93bf63399df46955eef8dd7a22cf3ab150f236db87e06cab1617e098983d2eb3648022027991fc8913b308e83e84f619250788" },
                { "ur", "168f244745129d244ac8a4e63ba8125485f804bdc55d9385b25786683c4e55f902c530ca673284e23c86c06996bfbeffa27e48425b2272cfafcbd89cbadb6384" },
                { "uz", "9f1a19a1cc8493cb5ca03ef2d91687e96643497576fb9a244cc2c0fd146f2540be30b254670ad6471e116be6caf54fa96f1876756d86b8123064e914bbd39bfc" },
                { "vi", "5ac9fb7f59993233a4f1457f18131a72d64d92f416bfdd06213b426e11a76347ecfeb2d2c874af2cecbba7512c9c696e5394ac1cc47ab94c3a0530b1a4458b07" },
                { "xh", "42df7830367eaa9dff9d49bd52331b328e5b0398d7e948ee0db33c904bc2b6e03620e8d974bf1e6b7daa6395b2aa628d19b07615be45e267d3de981f0cb60589" },
                { "zh-CN", "fbe12aaf723935d97c05b00049c31a1a412645c09af86ad43598224b0bbcb5f4b22b04b37b5fa5cff2880e9f62e7f9fdf289ef76b44fbd98f8b867850a5e5916" },
                { "zh-TW", "f4d9a723171eca26e07eebb3f45d5490eb4101c9c9e04b90742803c1009becb0a44f5a47af1d4981f6d4e2c9e6258e411f989004a2cbcf206d3310e86b6e35e0" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/129.0b2/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "ae175f217a8bfaf8c269f1872ac33fb6513b28e181aa73f7b714e6ca2825b43be0ccb375af6808563322c5469ac6fcb3220cb70f21d6cf959562ca9f5eafe524" },
                { "af", "f394b869c4b2138c1bbb120781f64f4231ab48a249288673824bbd0586c77a1c2a9f0066eca60254b47d1f309b000114ae39a7f36fcc2d1bad0fa5b0eef45ce6" },
                { "an", "4824a7dfc54f88aab9612ebd9f056b2c2bb6272380e3496f3bb359b855e54d249a8230fcef73b903285b9fb24efb5c89af8fb0d31f0b42f36a5f4377fb3f8f15" },
                { "ar", "42381705d03ea8453ddaa65f0b281f661e35953c0dd34e67207942eea2055d68eb4825674a3eaf8dae583ea664a123b8c284688ed5866a98ad5fd64bc4e3681e" },
                { "ast", "085365dd232936057f841eec53636082c150d3d16147b22564878d6897602a45326ed33da0b927f2d087f8bd5edf61a5e6e857a42b59d8f3ed84979d9eee7677" },
                { "az", "ed499ff27df674d3f7194cd551cbcc1992db7472294a3417149346cddb7de2113139eb5f2aad0d5b5a51db1602e25d8d1c21dc148fa9e1c48017a18bb944d6fe" },
                { "be", "7044c4aabdce05f732014e6b83e934d204929d71a85d0405b82bfb0f402ce1ef6d4ac1f9aa1a28b58c4a8070792243488311eb77cd4f181e2a9384eb4dfdd971" },
                { "bg", "01eb7c87ea8c958663ab131fa51a9f3147f9d48570e8b21e23143cbe1113c8b928009e8dfaff14c7769272859e210faf0f3576cc87a4d0ea950a0d70cf034a49" },
                { "bn", "6b11fe70510d8e02c71ec1b1687f5fd440aba6f8cee561462de159f3f36d687afe7bf4b812d4e8bdf01695e05c8bfc7839075d41a2d6328aa2b4a4a2c551438e" },
                { "br", "a5d509a1e7cb277444a8e27fc62a24df2efbcca0f6c84f9a42d1f1c1e60a09d51708f17f1fd91d2519538b2b3909e8d10680e21d8e4645f4548ea4e20fba25fb" },
                { "bs", "815ac96c891775fdc81aa8dcad49db49dbe324508a904d86c1a351f5aad9f4495cfeb4b655877c7ce153a5ffac00118d7f6152d531bd9e0182266b998cc8080c" },
                { "ca", "aa7eb6bc48da6403beaeb5bf0f56386c9cb37a6be2988a2ab0b222e55da945a03c58a8a9bf305bb02faaba724a2aef24695f2c8c07727bf24f7423e83c0d9b82" },
                { "cak", "fa7e52f1c34612dcbcf7faafb44d585ddaff2913ced7cf679ab48c6b3e2cbd9051f02c557faeccc539afb916f5e6bd7696d848f8c094b6abf96f620719b8ca85" },
                { "cs", "55f72ca687688846a4c52d71011d2f7696d4afe36dd8e7d2bfdc2217734d24c556c0dce87ec70ac6ea63df3811cd17e518d9f5b2a268ebe7158ba012a6d25c9c" },
                { "cy", "44b07449372e328d03ddca3744f8d84277890f5ed0e57af15baeed9411b250b42d78378fcd53be9f8e010fa68c5f390474d7e81481912c6e2c376bc71486ff6f" },
                { "da", "af00ccd1567ae7695e63e803f9f097594f9fc284ec6be73cb23cd7e857dc37925c306117dbca6e3ccfd524607f1d3060f523c0ad1ff2ee44c62fd900a40908fc" },
                { "de", "d7ab675bd72aba93134746da95177d76d1e84088b56732af0556acbcc8a2f9057f596ae112422ca950e66e72e70230dad2a9fd9b57d07350943ee6ccc63d7bf7" },
                { "dsb", "7ea964e1e657e67de632f9f219fd09bb4c30a31fdfb992fd99c76df17514b3751af82c1b6e9f00722cf77685662b9b7ccc528bca6050b41765bdab84edc6e09c" },
                { "el", "2d0b3ea4eafca57623d3181632c1e211949b04b6607e1e5fe8254f9f5d92cba52a6d27907ec31a5d97b2db24906bc8ec296719cb2d00ad240d75d44d4612ec1b" },
                { "en-CA", "930482bfc0fa342d6ef97b1310fca76ca42f67f646d5030cc35cc9bfbf493e39377574f570c538e32d085ff2d19cae3927eaf73903cf13bcf4988bc550a87cc6" },
                { "en-GB", "4f69196192373fd7d86ba60c4d76e7f1bd7ec7bd5eee8c3d3a2da60f929387923b19b7f048fca53fb362cc1406b3e2bebe7dec9abb4f962a8b29c54da48ba864" },
                { "en-US", "393c924a2ae550ad3d6308e7a5f48e8372c9edd0e54efec7595cf82b60eeebc01296ac5c419bbb5f90b0e702b19864c6970eccfe66bd939b41a2959f5efe60d9" },
                { "eo", "3c09eb178e446a1663dc58269666dc8f583af88ce0319e177c52b70c58309ad4a999f0cee2f54c82e7995f7e44bf9c43a9eb8e9d34ab8aed917633479f8f5b1b" },
                { "es-AR", "87295f025d79b72d4812bc3ad331b2276ebb0302178073eb9a30da2e6f45f3fa9e31fa3ac3e7e08eb9c62a1ae47dbccb1f645b581486091b4ae6a7a0ace0b8bf" },
                { "es-CL", "6a38d788fdc1f5e1ca0edb94634397e62737fe53e5185ff49f40a063f87d11933e82890560c0c3983c4a61e60e0de2c4a0be54f74434c359f4e1dbba30b082ce" },
                { "es-ES", "dea1b1083625da13cf4d73bc05f23c4b8c6787e887bb28970a2155ac9969dbb04ab815ec1b1bd5301f49d01267ea4fd4885ba554171668c61112a4961489794a" },
                { "es-MX", "c2cdd0bf3a39951d1103e8e552963c8d0566b61ce012e306eb8ddff9858edd1820477b323452ff16ea01a68f22f866b4a37ba888d65b35037824d24d7d0d887e" },
                { "et", "fec6a4837350a84f25b62651c6697239680c84d2b73e9dee90cfbcbd8687967eba5c324395388c759ffd3baa42e855f2229ee96516d756757fdb9b22bd3970b6" },
                { "eu", "8bd155c562607139375c89e28478a4f45530166d1878cfb6b3fa4034e7ea9368795c50345c4919ab55932dea4ac6eaadb9434739bdb259b44a1e119f3e49f26c" },
                { "fa", "af54fd8bff00be8a707f200f9d3d6a2ac28784cd573f72e544520cecb16ff1f71feebe1d179e99c911c6a3f63930693bb06bf2be8c07110820edcb595a8caaf9" },
                { "ff", "ac7c088a933527a99351ede0be362805ad2d1b92c2cee4be4faba25eb1fb117ca2f7ad9ba9f25bcacaaa6b4a1e9b3d23b344fa0217a59105b3c23555415d8a48" },
                { "fi", "4b747e089718bb109cd7ec83cc82da684106ca3b71120aa7a3a6b735ab9b8dc970215d58a27311188c864f9a0c2ae019307f2d8fd0ab63ffbc36607c075489ed" },
                { "fr", "dce3fbf190069be76bd97a276dad7bfe9c754809680470a6675084f441734ffcf595732d235f6817a3d907d0567d7665dd9baf1282da00b38f6848b6a38687a3" },
                { "fur", "0c3f6bf0d348c8b3d366694d84368b17d99c5012e41b17f881ee5d41ab8ced8edd993ebf4677ab32d48e1a806c416ea86f39678d75ba63986ee3a0d01dfc18c3" },
                { "fy-NL", "36366609db596482b9c156930e595b35562dfd850c2b0f13cb5cfdee208cb0ed9966eda9ddc7931828ef92cdff29fa0c49ac0e6b1b29fbddc3f30a46eecc107d" },
                { "ga-IE", "f88c1c28a1487c480d84f751c98cdded06e65aef3da28eb1dabeaaabe1a979f4bb6ef2b4516b2b47a6db5f0e4b592251e8281e4508cb505a1c4f84830cf267df" },
                { "gd", "f0da9da2de0275922ac91a70b0c25d5bc66028206bb2c0c0861154178cd32fce6a8db15151f6cafa6ba5ae7a070957dbe99156a36df11a404b8fd5084ce3309d" },
                { "gl", "eaf43c93800613fecf40ab4d65e46584a03ab0e00afd8984204cba983b62c4a8d78e9eb5b6b456210c6f1ab7a3eeb3d8088d6723629eafeec173c58b6a0425f3" },
                { "gn", "62bdeecc7f3ea1e6937ba586ebe37d333faa066498eae970988a9c20f1a465c6042a5b61950aade182ca652c6441a6822db533fa5fad9bf452a6285d58a6b606" },
                { "gu-IN", "d30ef0574737799abaa8c1fc207911d91662599763d4af975573ada7ea51fdebd57e062105ca27f0f898d93a643e632281345defa51350db4edc4fcc2a62c64f" },
                { "he", "e50877b463d88c928c6ce3962eb907a084a29d49379e9d99e03f00f8e80a03a2b7d0ad4f544ae73a62f92e04847b6374d2721486d32f4f13b655e45b861ad51e" },
                { "hi-IN", "2a1250551a26c0856b436694aee16d0a3d261f4b32225f4e1cb0cb5a37374fd35e65cf315f7af0db598b9a2bc511ff2d43130c99b9130b19936dfb30f8fd2c91" },
                { "hr", "f29b33ec2d479112d0f5b6da8ebbbed72fc643c8095c01b98f629ef6977e91dd0e4a1e35586eb4b1c0f754521dfbeba6db35d166d4ad2c687a47c4b5aa57213c" },
                { "hsb", "edec1728718cc7b0abd44320f94265b18c7bf66c7f5772362211fa9f80daf8a7ea5b2b566f715afd58a769239281bdf106efeb52f188cec21fe5b719b9cc81ff" },
                { "hu", "154a59af4acf5d5ba7f777cb74934056e44f993979ce5c5abaf6682a3e30f72a3fd70f4c301afc4ef7a7c9c238a480e6d624fc22d3bb857487414e244798e66c" },
                { "hy-AM", "c1ebf649a3fb9761fed84128090a60b784923f6ef6db662904fd1833f5b68c8db59db9f7888e6b81b929a161d46621523132fefd5adf8bfeb4e5beba83554c72" },
                { "ia", "d4dc391a09fa6265fedc39c146cfe50bc3340f8a463bbd75dbdec8dc4e9851f22bdfd2c5ae1ea51b3537c1699ed8c2773a2ad4e3e43c03ef2543e1f61cee57e4" },
                { "id", "c62342ac8654b9fc7cf2a197cc61c7081540bbbf002aef2d4462748ee7bbc2ddfabe8c2bbf664cc7983383f5f0dbb975acad5f7b364de0d81208659ff4bd9ebf" },
                { "is", "2898157cd6914557ba83a89c9feb2dd683cde3448123e70c9ebb01726246813d5bd8bc2dd76ef3b4864b09294b3212023a0e263561abe59ef37eff50e4ca0b22" },
                { "it", "0d75690a2b0b2743a9d0b5271c0fa7522150642d88fe2cc298bb965fda5cf34f167debb7b9792b68dbc49726abdeefc7e704fc35171077a317ae684ca4b9fad4" },
                { "ja", "a6cdff37cb6ac79191fa839b1a1461b55f4df990de34c9ce3a6888417056d5f2a164bb55ecce437710d30597fd7848146067e44f6a76450fce30b0dc9ab1d6c8" },
                { "ka", "4a14dcdcc80f0df73b2d99ef7d1fd87c99930acb74b9cc6be9f7eb4e6d38693cadd6cd0d4d769210956770c6684a77a5a76cbefb068751bb210e03b039b91df7" },
                { "kab", "d653f5e7f108b177cb43606af4e2853e568b772924f6ae6a6754bafbbeac6dc36d6b6b1fc8d952f9aa05b683ec18795268b24a5a23a90c67dadda185aae2fc92" },
                { "kk", "a55ab8409eb7ea9a2f7bd3a1dd58366e43e158cb39fe24f524b7904105bbaab982ec2a4080d0676d2f8270ab3d03dbc0732ef5fff09f6660e04e09eb76ec7246" },
                { "km", "67b9a9d22805234067dc05d4045092d23ee015edb4934c9716e151b3842cdee79d2e6edcce622d89f2925d45111fdbc63b9aabb69b00e60f1f7ee47089779046" },
                { "kn", "83c75dcf09e17378a8657ccb6679130241d8a1dd3ba3422edc5374db180cbfa58ffd45717e2fc89c070a7ababa1308d8d09c758f15ccabf566ea174e8ed734fd" },
                { "ko", "1a365d2f4992c68b6e8066339e9835e69a92b8fc90595f1a7fb98bfc09771c419963f65d67fef3a31d7b46c2cf82f739096e89e0056967de3bafc4ff95510e9a" },
                { "lij", "0ba0f8b6c84cac3f63b3d75191e6414f53b72784986819f032ddfc210836309170cd1b54b2bb6fc10c73fd024d4c28b0d36cf070e236ea4724209c5d4b0d5463" },
                { "lt", "b0255b9d6b4e0887123837080e41eb8e3cd8a4b687fe778f8ac40e00b8adfcf0837a7a9464b95dfaa80f9b2200525d1b37a39ea05da34524841004999af117da" },
                { "lv", "b809e19eff74329af4df9c3b7745474765caaa03d5b53ff6a76aeb117d498a99a66dd320f4c558d8df52af22e447547dfd0300f85fa9701b3b92f1a1a884d295" },
                { "mk", "1187ccd6778d14e3d18e5da087b468008bd17996ed50c48e184b08716cd55e1ed33f56ee857069c17a4d950ebb9158db241b5750f945ca63f5c33b307ddbd861" },
                { "mr", "3fb120665a3967184c0b33137ce4a9b3a91e3fa8059ca31a1d2206619dad11126d14f207aac99382b07235d3c4301aa51944d518479568566e91d2fa6510902b" },
                { "ms", "c1e7d73a724d32d8ed95af3c5b0b0c70c5f39dea280b0d6416322243c1c69484799a13b387d6f66a4f411b5b44fe8e1082d5a2a52e5b16a555a56fa4e5d72251" },
                { "my", "d10e1be88bde71cc908118ff9b565145149b6d92a49fd2adcd0abebbecbd506785f61199c3226cfbe6bbeff88d28b55278e1ba24716d1a2acf2bf905a027a23e" },
                { "nb-NO", "1494ad4fdf4820324f5e32fc415e356092ee99dfa860111a3dfb7cc79cb6e024edc30c0ef6d9377452dc5cc69ca8b260fcba49177e97d07e269c16a560d03a9a" },
                { "ne-NP", "a732bf17cd65906579412224175a801f92476ae8d0cfac3f1d6e6d020935a063bb3d8627fda9ea04f5db1bd5ae1b5f6d48d13e8fa8e2411932763eb034e93f3f" },
                { "nl", "779c666238ab8acf92eee9f7ca9536e92a483986b8e8d3b4fb74386a8d8a8269878ad05f21adc5d1d1ee7b5251c75af2a537ad203f9456dc63102c9a5af975fe" },
                { "nn-NO", "e328ed7fc9245879ecc3344bc3100fd50bb8d8da9b75410f1c08a0e5140b8b951590b89989ba98b62fa0544aec44d34ba989c217771d28c78e4288fa444eb116" },
                { "oc", "61fbb3060d24bf329a6a8de49ea7843dae17875ede7a0a48fe26f2a47df19e91cdc2ce3d3c94773529598a7e9e5876282b12687c46e9402f5b23ccea28b3239d" },
                { "pa-IN", "83008b24ac06f19238b486361b28577dc6604f5d502e2c320aaf756eeb253da3f9cf19b58f297e53e53fc73cdce329bc677404592c9c011338523c203fa10954" },
                { "pl", "51437c98c73597a6a4caebb18304c70f471886082ef24ab6c58429933e52cc008d8a12b34524af33533360e3a084e6738da0a081d5b33c38a7bcbd8dcfe44d62" },
                { "pt-BR", "7a13ce2ef8c5c623f003580be55387e1da9895d4e2b5eaf744d4a283250eb9f6ee62763abd03ec6b1236fc8ed2e501381da33b392b286389052f5e6cfd8c56a6" },
                { "pt-PT", "7bafd991105674aed34815270a27ea510b1049b5f667a4c869dc03bbe618f803a1ba62b440945765dc48035a25a38e6bbf8cbb3e2f0202ba655fbd3439498dce" },
                { "rm", "1226eda53742c21d5fc2f8cebac2bfc6b3483c52c04cf504bdd62f38f0fbc4e053094b199040a87114b73c432275ef8030fccf4a10c80abe7a18beb6f9f5f898" },
                { "ro", "d4d0073b41d39222ed2a6e80ef819d6d340c87ee151f9069f0c8e6570157854095b991b53efa7cd07611188da7cb6c065021933a12310baf98c470bb1527f83a" },
                { "ru", "87bb965716d8524df46fa51577dfb0e75fdbd59d2fbc2f4b0822b6ecba57690950f76129154d3851a4cb337dff997c5c8cdf159941f39a54482d573b0fe267ce" },
                { "sat", "1ffc78761e857654ccbc615679a49aba99535f08b5a742930fe76800732b4712f2cdfafda2803150ca1a86903862981ea3a32d13efa6151b8cabcfc2a63b23bb" },
                { "sc", "f91dd42f3dcf8814b9b203df39312f58bbfefe69e4edb60863661a7ec86d66daf96dc841aea64380961afe505efccfefb62902a130d103377e4e276d517feef6" },
                { "sco", "5bd9f78c826311aa7647062e28ff9a640574c82c73b928155a40d1fc6758220a6ebf6491b59995db96bdc99541ac341430d71954f8c89278dd4464aeeb1faa84" },
                { "si", "bbe0d0d42a17787c3354adbac6cb2c13703374d14a846604f7a42f811154cb1c26b427d45e9ce2ba94a01dd92b2fd22a0b3497d099ff221a73d0130ee897ed7c" },
                { "sk", "59f45714e8aa17bd0486337837654f01f7375ee20e6d86bb59e3090eb09e0e05724f565ff1523021b056a908977659c9c3cd3c7d123af6bd32e83798d872861d" },
                { "skr", "648cbd3c6b953b599bb913325b96259068ff3c315bf6bacb44fb3d543adb79cb10e928500890044c9519520435aaa5368aa55792861bafe3b0334b740675d885" },
                { "sl", "90183612871364fef4c20660a2b3586b707264a8488d6bb85e9ee887e7052f38aaaa072661aaa005b81d9af9248c72794077d13a19f3b9b80c9d80667b9a5b59" },
                { "son", "8dd08dab9006fe7433ee9d9e68edcdd5e157e8dc6fef24a777bf6b1a5eac7eed19f7b9ec24208ffd66894f3ab892dc36dff93f7c3ea9d3bdc461416784aa96b9" },
                { "sq", "e88460a8461993bb79a2160c7cb6f0ace21ddf027a37adf5cb75548942732ea416b46396a16855b1ff134c76204c92eb15f3f5bcd1390b62a00b831b2b7e3d9c" },
                { "sr", "d206ea63c2db346296361a0d4c6d04d7baf06ea2cd5c1f460bb81e88d63226efa84e1071a7dfd41ad0f6ba44be52d419a4beee18b99602c2aa9a413737b27227" },
                { "sv-SE", "5fe0d06348888334ff9521702fced6a28b48a8d67358537080e5a89a63110a18e6fb4730c1147241a25ad68266e64247a9d49a4c1b6e6173b3b48d659f575b9c" },
                { "szl", "330733a8595b2b9efaeb9a9e038096e2de813bfdaf2e3743fa5d5a136cd460a4684cccbffb7de0e23769d116d729e5b1ac101867304c04ab2e40ab74b8537bd9" },
                { "ta", "59d2daed94e1ce3a6cad6e45348a1baf528aa559df88be3111b0c09c8ee04a7d42945cc125e2a355655060a3ef8d63ce73f707eee5bdd073b4d5ad787c50e204" },
                { "te", "2e1cac135c828f0e9fe5bf8ba10a16835a8d01d6f030b908232f14450e379bb08ccd0b8e5307d60c0aeb1358cabab6e21246b6565401cd9e74bf6f145367576c" },
                { "tg", "f1aa6bd8db5cf3b614a490a1717d04bd434a9dd63d8b52d2111fb3bb49d7f3dbacc7ddd13ab793ab127afa5227d8804e3380024f1e5125d6fcb28dbca3199ae1" },
                { "th", "200c930d6c4b4c4f4793167c87cae211a64adcef0db1ce84872ac67d9784153c745f5a1385277c403554d6df8e9df73aba4bd72c4a2e6d67c5c4432dfde61fb1" },
                { "tl", "8d34a2c92818a7db24f20fa03e3448ba33946bce7528d9277a1f834625cb8aac4fa461676445f856d185ef5110f57117fca59d6e6a45e2759fb53fc46384f9bb" },
                { "tr", "00906a4334d53b244a3806b915df95ae9d4bab83219c2de26700d11efec35225eef61bf526d8e58fdff4c8f416c0e04423b5a98c9ecd8043887c0df5d1ba8a64" },
                { "trs", "e87b098426966e7ff65355318ebc829c97aad89a9f96049b28b1a603d2780619de3ddd349b5754d3865a2f008fa36d84c859f40e9e16d48b09b1d25e3adf2350" },
                { "uk", "0bba039647377e8faf9677b106355765b06113f32e518a13324f76306c0b2a51f0ff2c24aec2a8d12b021f62ff926fbf6c48aad693f4f949e941d3fa075671d4" },
                { "ur", "6cbc34907c430d8419dc6fd2d602eb19dddfad84b18cc47927e743616eb4d3e710cd147b8acb51420d0890c64636dbc8b1b44a36dfe0a3842528246c8c5120ce" },
                { "uz", "12b5737a9bf048237d5f3a9c4bbd8ade131cd66692486ba3c79f6326c6f71e13aca6925a6e135b93b5f74f173bd8c2ec8c61dec7fc784ad2369265aadf8006b8" },
                { "vi", "25f7242f2def712a7290217339cf405429906ca0e24665c575428135a9f47383aeb6ce0dd8a5d9307b7def2125e0ab5d43001f29f9883f8907ddfb15eedf97c0" },
                { "xh", "5c246db2343e91046a15bd668f238939d39448ecf1b98f26f17a71eb4f5816c45655ebd703cefa282a3496cc738b3a125b4bc3a98ae4a08f778d49305136ee0e" },
                { "zh-CN", "675805a4de0c16d2141b1dc1df441d5e4fb1078b59a2bacb035813456ea022ae7ed5c84edf451a5156fd2d6dcefd4c906083324791548f6b92d814477967bfec" },
                { "zh-TW", "4ef1e1450dec8153f099763ac677cc1429a5ea6b8cdbc98e93258c73b1be6eb3c5d837cf38e4c07b968fd883d4ea55a3e0cc7d227731e846c3dc53367757d1ec" }
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
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
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
                return versions[versions.Count - 1].full();
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
                if (cs64 != null && cs32 != null && cs32.ContainsKey(languageCode) && cs64.ContainsKey(languageCode))
                {
                    return new string[2] { cs32[languageCode], cs64[languageCode] };
                }
            }
            var sums = new List<string>();
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
            return sums.ToArray();
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
                    cs32 = new SortedDictionary<string, string>();
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
                    cs64 = new SortedDictionary<string, string>();
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
            return new List<string>();
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
