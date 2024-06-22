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
        private const string currentVersion = "128.0b6";

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
            // https://ftp.mozilla.org/pub/devedition/releases/128.0b6/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "7d6cf2cc1713e254985d5be81a717669956ceccd8f95ab40bd4a59bb3590ee185c24057d02b63c37022e57241ca735c9a8823f2b77757799255fdbd36b89cce6" },
                { "af", "3c5dcddedc8003d427a957fb1db59d90befbd0bb801c61958266107f2d71514d530a86ccd3e1251668718dbe6c7cce0d1e85fca17bc0afc5b641c546ab5de9ef" },
                { "an", "e06fb55b0f50c14cbe054bd64a1897fa6f380f7acb71c6d8eb1027995c4dcf1fc0c0d8acec3415f2f2689df924f3ed31e5bf10c164b05e6470fe13903d793cc6" },
                { "ar", "c8ca64291800432296ed00999e4d424c9cbb747f1b145514b45d0cafffc0237021dfeb4d8dbf3ab79cc33f010550cf3d943064b12fe3d310872c8d1b699ef286" },
                { "ast", "eddbbc0ce7929c0c03d6b2673f07a73abf133a4ed26c1eecff3b73bf644c8eaf626a9719b74654876ff5e7f275204665ef473ca28cf571146810f48b1ae55512" },
                { "az", "59316191d541a35dc68d368f81b508c7b82ab764612251055ea61e3ffe884e32bb1cecd47cef738fea958a2e60c2791d0a7e28297e05847c32537600c3945102" },
                { "be", "c6b2b975410175acc443d6103b25e68f2a8391d8c92c65aa8785252deb425379fd0d29c18ea7dd889bfd993157d9b515f04fcb191ed07e4763352732f996b2c6" },
                { "bg", "c516c376c9ee154a4b90c0469bf592b9f0cbfe89f16983a280428df8e45a631802b795ed512ad7e2811901b94cd110e1fee24455ebcec2ca04f716197eae961a" },
                { "bn", "20e37a3c5d24cde7a56c5b01ebedad05a71089790b63ed4304bb0674757b9296bc9fde91d32aa4f7e3960ff4980619cdba8d0078994511304aede7c7ca107922" },
                { "br", "997f11ccb876f2a6ef2f850ffa6163e958d8bb2d1c0470f57eeeab9be0d264f1310be56868e2ac7f4e510a8b2daf16489d13840f78b5ebaa2bffe8f005732129" },
                { "bs", "66115c95076dd2c4de997cf4b8ab3464f664bc1b3b8db9f77aef23b9c60eb87ded91529b89043b5cc1c7640f10172cf68275db692874b1315f928f4b151155ae" },
                { "ca", "7e2db3c78c876c76f93413c1aa8f67543fa1f01f0030436eb5dff2b335a3b82bf8e014b2b18b35b6c7dc3361bf999001dcdfba2bee90611268b6f8dbde09ffce" },
                { "cak", "1ec98107df193d8647b2942cba508986cbdc6aa11f2756a7426166b915f810b79c84c8069ff4ed760cd0e72c7513ce5748048c550d59e206288eb36496a7b168" },
                { "cs", "597999cc50ec5df15abebd87cd7bbda53c4142cf55d941e1a9ba1bc9a633591ffa4190b0b45d7c9d6d71079b479746c8ad593f5939c81e8a034173972ca8f8d9" },
                { "cy", "c2f8a680c2d2288eb5ddba4ff2629c9f589bbacc4f2ab18f984b54f6367b67903a7a87cc5b1b1fe29b3af9080b5a0af96a55a7da1c8b14cd455ad1ed690febc2" },
                { "da", "4664e6ff402eea3c780e7e53cd4aa9d849aa307b5c6f92f6c96ad1db6b5bebe8b9187b381153f621313c888f81987d81893ef64785c2a1ae62b145320a819c0e" },
                { "de", "ef23f56768ed782457b45b72d2ce94989801b4d2991680a2a5a06f94a51afa1d607456ed9d68f7b7890ce34d0ed9cb5f2b1eb3217c67ded9392543e46217428b" },
                { "dsb", "fd54ebdb146a8555a10db3aea08de47969d1c7bb2ffb59f60ee62b18d771b9f1151d13d78fc75439a8980ed35882b51a4c473b5c72378f66f8b94390ad963214" },
                { "el", "1080791bd660eb875a5ae0f83bf1e0a123ce8dadf2ab18350e3fff686e28bfe04b3ace39a21e023e8ae59bac1c20a52f29f0c7cccd74bf6f9a6b4aec6cdf8997" },
                { "en-CA", "56dd1c5c90cea5231e6ec9f7040882a4bdd54416d092c5b184ab2533648e83a37d5e50de1d95753022eb717c977023aea43659250ebd5af17d06c6ece3d02c59" },
                { "en-GB", "87d60f9e2edb65942a216260f3cdf2d0245b71818aabb886ce43d8a3af6391c729a231f21d43ebe9e63bb8c4d64608b1d252c72cb88ac702a3f7535c9e62d442" },
                { "en-US", "7e08a6a221ad16d11bf175f99700189702bee834749f2200761c0baee30abe056dc4eb58b6a6f686b25bcde701262a5d87700a4b8719ee4b74e169b0cf1be7ff" },
                { "eo", "9206ad46acce600e87956084475f6494e621c20a048ac157d88ea29003a18c7aa5b44e039c4db13e2782d436049f76cb13b281e7fff17ef92775d3ea51771c37" },
                { "es-AR", "91122f1e2b2d4aaf298899fc8e0160098ac0b8a7b588449bf9e003f6c24c34641049387a149ec62c61669f60f45bf629abab80bd7add51422fb173a3fee12f71" },
                { "es-CL", "5392d4e589480c6e6b22a4612db799c9b583179c9318742046db1c1c4156584bfe7721af016111ad99118cbeec3b94b118e0ef39750c9b767b506b1a43b7eb20" },
                { "es-ES", "6f755a738464ac2ee18a733ebd538dfcbc0a7783f82891cf84a659e6b5dfca817e37485944e46cc038835bf4059c1a9abea3d05e0e57eda78039c62a7b8d7e5f" },
                { "es-MX", "46fbea677789e6b12c13673f4e1be706688f9698e7fa8de15385186157831dec94b73822b65ff580447da58562cb7f8a6d37814551b9b9e27c9196c3e3228807" },
                { "et", "7485dd27aafc7be81116483d343c433fad528797e4a46be2efb9fab9097567a005e7ef4a35e360ab8558e8135228a5daed2ac73ad79cfa71b7931c846da7f820" },
                { "eu", "3c69c63667925bacb2dad7afb572caee8227407a6e714a576b7cf42b71616fccf89ba3c8a0ae8d53af5bb28e7df103ab9e22ed557a847bbcd5007781a039cd33" },
                { "fa", "c4a54998a1b232b55bf669132477d40bf76c73b2aa6f07a8362c649276bbb1d6fb466f71a82787a4c400db4692cf6c67a001f84382ebf4432ab1be10bc3773ff" },
                { "ff", "b9efbf08f36ec0505c0f6066acbe37ac2ed4c42ea1f41e48a4dac14789ba37af3b020d0925b2428baf333675f764eff72a0e59106c364e96373b1e430a4b35eb" },
                { "fi", "b3122f1cb358e037ff4e2e8680942b64788ba62b6b4c436eda96329ec555131fa5c2c15622047a583c50feac5f799e867e4ed9c765f2539b4b6273098d56615e" },
                { "fr", "6b1192822e1ebbfadbbc982cafb57f2fae92f321e71315ff0a5c73f447064b3339ff8f0916988f295a4c9d8b7f4d16cb5599fec08ddce42a3d3572955ade1753" },
                { "fur", "751bb458e09712f040d88c3e7d27d80d3ccbb77e83ac3e6dd4206b183c7e6448fccde7f8cef2fc69e2c5f2d69026bf8832bed17a24c65876a909debdcad552d6" },
                { "fy-NL", "103789894de5bd867181a21753e6414a81464114308af6db6989a486baa89bd2753c8c4a3958cb927a4a03e3671c040768b8fa13b00c5588ad54806f9b5d2480" },
                { "ga-IE", "2728084449c24449ff62fb8d7101260eb4d12c35282a071e2bac44f15a606b399431b953ece7fd5195cfb5657c1f0a8a40378e28403c2dd80175bbf4c00e876c" },
                { "gd", "3812a7755cdb5878ee68baaa850029600a1b30ce6d29f0f324eed8960e5ea3eb6a782f728d5c12f316dc897e642fc91e1a07ec8f4cfaf760bd85c3404a24b615" },
                { "gl", "93d2df8249345781bba47568e8215850e7651707e3f456b9ee0d98edee429bf03a4dff41d6d1d14601ebf4d3e5e5a853df9237b94eff371760ddd89563a36b9b" },
                { "gn", "33e80ecdc227fba36f43c3b800b14820c7989899f371d37f445959e7c734200da2e8680f8f56891a8621dab6bedc0640ed1c781b767a8efc778701df9b4cbc3e" },
                { "gu-IN", "5af70425707bf67a5573eb5bc239127ad4dcc8ba7037407724f0be0d8d65e582d4deb3aabdc8403e5d5ada0c0a0fb5605b9814b5e5e2f15e07d77353bcadf877" },
                { "he", "a3a323903660d398739322ededf540c27ee318293e84325aadf891b53dac6c30aabfe2d4699624d2c01bad8147635be30fd02ddab93e440c3fa3df65479ff1a1" },
                { "hi-IN", "4bd535a614b7a4eb5d1dcb76e40ef3aacf44ecd73e023502c478ee12bd8e1810ed55122f6099b8066a451c5caa434e622aef8ef066ff1854f234e641b10b5af9" },
                { "hr", "c5a18ab3154be22d38a57d326f7d25fd83a1ad315a45b6cd22ad0dd8dd32542493b68755be6fc234057c8ede5e043c7bf7374f96c4cf14fbf130169ccc5ef779" },
                { "hsb", "556c6651518eb7ad526e87bc2a11aa9e38b688250f919bbc70f405b672a8cb1b3b976a5bad0d5f22f2f41ed8b2d1cefbcb8be03e455463d933fc81b3090aa7f4" },
                { "hu", "4252360b026d9fefa778563c4e3a3630838be7b3c5954c827dfdd1f47d79f490692b7d0cb233bcb41fcc60839e8c51812475c3eabdc9aedce38653b243156964" },
                { "hy-AM", "2f9679aca124570fac2a1c7a23b701ebad69fedb16da0e5990f3289c9a456790cca39f118e811dfd2ac73a19a4418d473d569627d41ba277194949541eadc51c" },
                { "ia", "1fd00035f7df68cb7dcad5a13590f306f35b34cc4f23d0a07f977ca024379b0a7b589509a9327e048fe46d4bc8d420c264b3a76a688e1aca3aa3cfaf19f94709" },
                { "id", "9a6d93437f9e4267d8163a7044b5f5863af51bd8cf4be6410fc01de2d7c293503ef1483ab81e383519b3dc2ce9a1409485e94625bde0a2b6c75c78f2b9591bd3" },
                { "is", "65491b78f9e1207e537e42a32af5c22490f42b4edff100fa9d08dd1a19b00c63e97147130b737487117bc4337d60fa8f7cb015631c15e1fb5ead229cb526c5d9" },
                { "it", "e3882c85d7e5c6a5c667da332a986105a8a507e2353615ea85bf931ca0a89553563a5366d21b522c67cdc1c3fb6647bdb267bdd1425bb1ed1051a6e5a9e93de7" },
                { "ja", "09d5ebba9bc962639342402e239f5182f9309e22fd59371dd0e7f6ff0b7faf8dba80fe74b85797695ee1e3b87ac36b40d3d51fde65348c3e251e30de88e6ae2e" },
                { "ka", "64e1972808819da770e54e10c2495de280d0623e26bce2acfb6bbe9c73207b02c8d9fc8757bf128dd1dcd364852ed8553e846ad1f97359bb7144a01b24d10803" },
                { "kab", "d969c9479718f2c1733d3ccf3846157756693478337d8e20e6ab73aef484c3daf8d4e8013c8fb7b4e9f8208aa8abbb4c3a8df649af50e1330135e7d611f08bea" },
                { "kk", "f90187ae5747f7657692ac49b09eefd86afec1e541d1d0f4bcb7608f37c28009d6b3e38a3b6f185ff717966415a705c772395c886a9906a9cc589cc40d1d8957" },
                { "km", "8447b47f0824d7d0b27da2b0449a225087326e75a431e240d882df0b080159e08444cef04a8e7ef41ed54f813133d1485b0538942766e1312c90c49c2bf3927f" },
                { "kn", "0b0a6357ea8107a8fce4b0e35b026696da5c979c9013c617576fb834f3594f6182b4436df34a1d9c56ad41c22ac1f4e13993e83cd874f9e8601db7da6ec2598f" },
                { "ko", "53fd606db8622d30634a70a52ee325c7b8bf875da956985748e23eeec6ffd0d9342574acd08303b143ffdbd9579c9167b937b419b5d81c4411cc8958281767f0" },
                { "lij", "a4c46c8eee5fb02ea959a9e3bf7ba98c0ee0008f6eae53d894f8a888b080b6936d19e725620c1926c91e7c2dd9a49d8bc2df2cd9e4ef7e4d23dec3bd32a52466" },
                { "lt", "049fd7f114a8aac6caa9c5ac2f2d2eebf8956ef4cf4a6e53ec09e793c2a1f16692b84c2acad715084fc984d68dff0704fa9e2a576f5124ae00b3c6ccbcf29848" },
                { "lv", "7e80b95472ed971166954b19d04c9af5ed7ed6101946213fc84d74f446eaddda0d79f412c3c6ec3789eefda0e0e470d953a704f047a276f74812c189fa77bc09" },
                { "mk", "57a5fd13f3f9be4355ad313c3d6c005a14037ea9c4a79c535c4037062dd843dc9473087315b774316e8890d42b974b3c01a2cb0df2fc5a692ad5b3c0cdcaab4e" },
                { "mr", "7b07fb4bb3d98696ea94d90fc8363383e13fae65fcacf60bf26e605d7c0c40c07d2bda0342b0eeccbaec17ca8915102107d016ead8101a619c457f88e4b00477" },
                { "ms", "11e3e09758e5c824327097aa27a12a13b4a41f26161619deb6751c0b4476100fc797ed735d7c43ea914d88e670fca096cb7494e1ac3175dc118bd2e3b040d24c" },
                { "my", "41f7c371cb77c087ea55bffb39dc3a3db35bca3eb0241b1b9f7faa7300a0231c9f4e29d162cca40e640b4e07bd88885a15c8ff33c152b8257de5c361a54866ed" },
                { "nb-NO", "4f622cda677a855e1be4dc61dd07c6d7e895d97fbfe9a664faa0b1e8f3ff7cab011766994636b10aaf7a553167712a5578f24db886a34cca26a1cf4f7e57a129" },
                { "ne-NP", "29156b397d3d85f2cf149da5983010f91240094f257accf6c42ac00f20696279c99364de5fbf03be33907ab060a54fcfe0c439e99afbb9c453336a22a3feaee9" },
                { "nl", "f9754e9b5186f62cb44309dba4eb985a04d928b481fc8de671320eb06aa2fb04a14fd3b4d57f73f41d772e5f03ea7a0e2fea979cfe03a515dedf556d266295c5" },
                { "nn-NO", "34e92a2fb6aa34a7fdd4c5b11243f6ffcc1e3619bbecb8105b450d4f051647015aaaf4e53ea7c73ec4f3256bbd13c8adba01c04e4765ec5da631ededdde913f9" },
                { "oc", "d41e9bc9bcb757cc73621683d8c42929986683462813af74ff06da2e67ea3fe987ba7d9dd9acb1ef95cb7a917f73aad964f664ef2f8a539bebca2aaaba61dbf4" },
                { "pa-IN", "009555f145fd1f45223155ec2121cfa4f9b87c45752ff0461f8d2e6b2faf455ea44a4f05b5810d3a40541bdf232c0e9fb286bcd5f84e67d4f3bb6b1f743c5bf7" },
                { "pl", "d0c9f7160913c06029424e162770af5a388f3b256d5d410dfcff71a6ef9f20bd5b30324cffdd627757f47c60058b62291a5228309b34661a89d6cd7e47fd72d5" },
                { "pt-BR", "45cc3404ebada6013b3fc18a9c0aeb0360db2529929e73add3c135df9b6e8fd67c3fbb68bb25e052316e31a8de37bea5fb8dacb2d3b9386308dda5d292e04d7d" },
                { "pt-PT", "b070d56c9566792fdbfb3bdfbf90d268becb728011034130cd2928c67bc26f0dfc8ad0f3e1f59686d915e8d33e31cb893bcf1fdebd0eeca213cecdec0ca4f777" },
                { "rm", "d739639d90134f9d56cfc433956d8e0a288280f07f12f22882503ce01389b82d5a70772c295d163057cf66816a15819046026a7a3f6ec2b93399ff18127c3248" },
                { "ro", "4e788fe89a168c00e1009ebbbae9cd768cbe6509fa0467132964ce1693be3fe83a9d0212bca9380b67d8ce6772c1ea8db6ba768897f34d14aceb00895e9e90ae" },
                { "ru", "a0e9c968afea8211dca3c03ed8df50cdaa0899f8d612caab64cd9194857e609523189352028e374d76d57c219a49a7f55940aeda3a40bc672f71c1176fa8e50b" },
                { "sat", "501d90da60d84f7cb66ea2d2084e347eac623099148013d87b9a3c682c480cb26b0b7cb1e66765e2de8a3f43b8ac72876be9bfb4998040426159f4b8a21f4196" },
                { "sc", "9089aff6c12ab9883a06308d70be50cc324a1d9a779a5a0ba5e011cd7651f8e194a00c03fee6b951e6c172d1deb8a9e5032f91fe0cd8aac0a8d4104042871ce3" },
                { "sco", "eedb6a5f14d7b6940c8a5852d6b9d0c92016ad55de9b3e55c78d9ed79dba662e55dd51e6ed97f567f674dd28659fb360de4b91e44cb1ae167f8fe52ca23f506c" },
                { "si", "8e066a251897f1b8251ed4f8b7935cd209502da294ae2a6adfd6b279f5ae9fdbbc818cba9f3c6c5c0c69fc9022a343b54132de1d95861a49f92c85b2baeeeb3b" },
                { "sk", "6aa31184d22b750a3ee68b39ed7968815d073c72b6dc633ec525bcea55ae24bd05e45fe91f5969bb37ad3f60e45e7b0c036b4ad94c67d28bd8f98d88aa121ec8" },
                { "skr", "73a73fb26552168fbff287098bc142214c4a652401cd92340152a3ab46a60f78cd21a877ef5adbeee8a8c07624d61fc855bad2aadc6c453ed984d4c4141c333f" },
                { "sl", "906fbde1cb2b8441bd69a8ef614017d266d5ea8bf1c9c8d1cd5a03d6e8792ebb54f1376c23dc5ea9857e6db5658810adb7c10b5c5382a0df650d1847ae44f473" },
                { "son", "0a28ac1fa33cf316863ca9fc8124e8791bd0b3bf50833b27211a77793c5d07e4378d05de02334d2c7ec08a915febee85ca04a1da6d61c7b60b722c18cd957bba" },
                { "sq", "c9e4b47b045d9971f9a134ec64de5d61184063cd0012b7f20d8e6a84f028651844b1231fdd86f71b303393a5abea12125cbc0e97be2427566486febba84ed7b6" },
                { "sr", "4788b07cad2e2ebba57090b3aa172f2642f90ce7d4a428d25bab468be1df4ab54e4793d0848d7f677462c116490d69894109d6709a0c12a89858aa2a7050699a" },
                { "sv-SE", "187646801857db15bbd2bf27d386636ffda51028c8a6fc93137a68ef8733c191e61d21cd369decde05efc86ebcfdd30bdf8e7803239ea148bae354e17d2022e0" },
                { "szl", "e4de7ac270265c4a712fe7a5969ed5eb2183ea4dccbf4552856e129ff566ab05022c34a4559b637d15de251fb72472bb194abd0a586d0674ce41a23add0aaa07" },
                { "ta", "79d05b9bb0a76c82f8b0e3b01873a2aff195eaef60e9bfdf4421e3528dba818fe8d4e5307bf464c01730241451a864bcf31bda437de976fcb34346b9e4c68851" },
                { "te", "65acf42bf72852d09040821c042b93d428f0b5f56ca167ee3f2d2dc47e104817819721c8e9f39160b0cc73ecf6eeaa739f0cda671d4ce65d284199b6dec07e27" },
                { "tg", "ff12162314ef589aab3a52ea79ecae03b749541196e82bdd25a96e63e4392b292ae8fa38d0636ed943886bb58964b6f2ec5a986e421a29fbe814b71a264847ae" },
                { "th", "c17e37fc90041e3af4252bd6cb591691fcbd1e4b2896b2126802ca839f38b28dca5a5b7fbd28e3ad5b9cc1946f1a474d4b4dd4c70c35191f2ae914a4cd6a2c6a" },
                { "tl", "4ad03738707bdccab69b15b9769391a15fab0ff72681cdf57a99ba1c7171756d60bbb979ffc793ffaa785ad3813541f12d8060611b96664455205eb9db5f22ec" },
                { "tr", "1dd77f41b7265ce4fd21993713136053b7284950c3e5b209ac66bb7c3e2ce7d6ca1748e5804177728f55347f46173c16c6ae5d0fbd76258701e012349fdcd062" },
                { "trs", "f4c9d03131339d9cc4ee920de96f00ff25c76eb0d59341f825a7013efa589bba21252ee36643773dfd7853ec47b3e9cd961ed4b699f58cf5f4da25bcca725cd5" },
                { "uk", "4342cf24b83a66a71fe263542c9c66078a27bb2611f57e33cba1fcc6590786caa5b52f45ea8a96e9fdc6b24f68590d846e45cb45860ed415f17e061a5ef7a8ad" },
                { "ur", "26272978934f8fd4e16c7e0c4dff3e56fdd0fe66fbf6b6d5924f5fa7855b82270773aa3f5bf38d916b140a5877ecbc73528b2a0eff06157ee1ae9a46a6c3e407" },
                { "uz", "90ae8ee5ce1073fe2be4a43fab748e6b9244e856c5471764bf148d7f6748b557b281b1747e2c1a3e8872a251930fb049194448e936c1640e2b7e9079ab553935" },
                { "vi", "350789eae5a9a78986eb750caf5e52e12b56442063d47bd5da26f73ea9f57eba4970c20593d2da599580157ab2e7048e22470879ad5d839d27ff55fc3ec87bc3" },
                { "xh", "83ede75fd0197a8465dabd9ae8e309a5dca57f6f71da3abc727aac111da4159ab5e64000b180b9bf4c103d93cda505275bed7f3fd0a79959d0100fffc47cb72b" },
                { "zh-CN", "a8a0108cba54fcffcfab4c42706c0667653cf95b1e7ba8937c348b1372806f013d8425a2cee3afc78eafac629644af379eeda3b51cbf690394667b0cbf5c23e8" },
                { "zh-TW", "953ef3dc9329ae67bb31280961720f588e60c0e81d5027f28666f52ed04ba1f9923e2bae514934d03fc2835270d57a07b6de41baeaf4fcb5e5f7044aa6201fd5" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/128.0b6/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "79aa6a093ef7b614ebf9be77ebb94fdfbcdb490c21365a92d7657e94a8a11efda490898d2c3bc217a41d4872baedcdcaed4f4d9c569840ee58a09d1392d3b669" },
                { "af", "fd1ff3a740149a0a8a8b9eea020498e34ea0e71c55dd646f1ce5e9d61e89191146983ea39744bedf8760dc4e6b7f62795e9a9b26b424f0cec7b225249114034f" },
                { "an", "301f072737ace1cabb9e1a180cb58438fa20b744d77b4e9cf4144f98549104a2ba56a00af7e77812ed4554044ee0e4227100145406d7730cd9283407ce8224c0" },
                { "ar", "47b211294d76ec9f9ecb64dda180d0745c11cfe78d6f9e41b0332b4f552ee05e6c7ee92c1cba6f6dc1f9da2606d793f2c9b3c93a450af8fb97be2e275dbf627b" },
                { "ast", "38636ba97923d8d8595b0e1d809cd9077e21996a4a038382b7bbec775f5f2b6701a2027a666bbeb4ecb8a351414e5c4237cbecb3818079489e98768255e99978" },
                { "az", "c1fc84c4c09ea0a020d8b731c210a041413ed45899b589613e5163ef4519b324ca0d2d68eee660bbd2e0c0bca3fb297e93636f82b1cfe98459897f80e0d52fb8" },
                { "be", "b70cedb8fe7f3a79155adaa3f077ee88260ccd24ed7f1a72330b23cc75363447bcb1e86633edb83d5770b2062e59d3177230425cc297efc059c620d0440175bf" },
                { "bg", "b28c2b5a7c620afbd641d52bdde4013dbd91ee86379ceb974ed7de1749192565ab68b07487a7e708b37a073a19ae6535f55257cbc929683bea477502ab298775" },
                { "bn", "a9c6bb36ff3698e9cd49b6bf5e73c3dbe9cfac320b516ad6fa572ca3f0af69149efad8ebd81fc9999cd0b30d233c7c2dca6e91cbd1ff92742b741ed5d9a10e3a" },
                { "br", "459da58c69d0ef9eeb9db72b9dd9b0cebda1e0d878c6b84b8322082a8a36201182527b0acaadc87fd0b21d06e8519c13c08a79137000982c22b76bc179c32071" },
                { "bs", "fd76ddc281672b6349ba1f6b8d9b662541ad713123008ccdf22ffd945b67ea7bfd1d449326202333ebee7106627dc218230a632f08fe7ab0a17b479055e65c61" },
                { "ca", "b04e1daa550619f63bf25464242808cde2f904d294545f207add460f12e3055c3625814e3c3a13ef9a0f886e8c3ccb1b89fbe6c6b043368a1b6ec512721bde66" },
                { "cak", "6b7dd0c16ccbef1d2c28786591a9d65e2355016dc62901164951987781677a2d1f96b1d6c69b703bd1a7a7d9ec02113119978b15ecba16ef5353b5da012052af" },
                { "cs", "f33b4b572686b27903af1a82807cf7087f36f71b630c8e11394b6d621d349343d451fc640f0cb489310e1ef6df2e9123c25101d6ed35a65e4f357a0bf2f48247" },
                { "cy", "bbc408c1d29e91f972a1f16e0b32675d3fd156df78709cf6e2caf1a667a2eb113016de39b27ef39f4ef9ea0793b6eb29c085441ed3072b7bbb24765726781dd3" },
                { "da", "9a4cc4bda92b9be805dcf7d6cf961030846e9deadf31a1113ed0d88975b7205fc0429cbe5767061cdaa5fcd7454bf9108a0fa258f4d38255c3f1f8d6a16610dd" },
                { "de", "69cca5045e84c9f77a13aa7f3089b2e9483a36c93a1d27ed6f5cdf315e589d867c32bd5d050432f22e28dbe3a4ee55cd01b784304a5c262bb57bf62f2d56da22" },
                { "dsb", "c1c796a76cc175f47396d6f846c116fcc3f9edffbb9d6e850138743edcfb4f1b726f543ee323394f9418145a77d4352b85817ad9cc7da35c63e8e6efcf6b5e70" },
                { "el", "5e992eee8854ca7240016ec0633bb17867569b207a54a413230c3915e516cf2def90d225d40f6ee4681255cb1f14c4793874eea6e0d6445e9eab3eeccc0a7586" },
                { "en-CA", "5264e0c8210fa083f57f0d18a2f714287934d5db57cf1c4d280437362b509a528e618d0d1b70fab93d9488ff0f90bb21d0f7d15b70e718ca35f096f424da5cd3" },
                { "en-GB", "4a69e92bb5158a0f64b06f57750bb5d076ea473582c49596fb39523de8d923bbe77200836e40f4a29fc810f77f8a163078c3cbd2c24ea7cd77c927bd1ea77153" },
                { "en-US", "a64f202a97bb9d22ca82d5000dd5e5d15abe06829e6297e885983798902a324a0f7bc5a58833b55045b055e680581dea8ad726607c38a0914a1b542238d3a63b" },
                { "eo", "93caebf120fb5f1df4f412aeefa90fb97e904abf65cb4f34383cb4c891b74003c56d70d83a2109ce6a7f14b76afe8656942edfb5a6c695b076746e24e07e0957" },
                { "es-AR", "e17cf23e7457e9619735a175b745df9f78dd2f7590c065680ed6db623a0146cd37c99a5de56bd2b1de268f4b9dfa1e9f96700b30dbd26fb1c74e73d384d485a2" },
                { "es-CL", "1b087bdbab3d18b78e482924f4f37f2d91be68ccfd71832e081eefadf49186b6ba728460e053d9d0acff0d8cb7f75ce147554f3724d06b5bdfd577d08ecb184e" },
                { "es-ES", "f5f865a1f87f4badd10df642658764d806206f685b4dcea1d9834b52543a2f30a9c885b611183e3ca2f5b7c4bd98e5a46d21b468b6ff73ef69f0c2aaa29e1371" },
                { "es-MX", "c42499bbe1f68609df041c6c89fb23fc66f51383cb4eb4a1def9b36cd1297a6e6743dd7d28d24394e720bafd7c2e00202f6e1fe58f978510bb1d344f770c0d4b" },
                { "et", "a73df29adb534f42a2d59c9dca6278ed08ccff7a7ea4355e482e6ab14253ba435dc9e9611bd0fbc8d9b3e4689a5f132c238c03d1e1f395a66dd74a8bf7f59e91" },
                { "eu", "e511c8bb53c82419d766e7c7d8d14214d1e7c4a2159252ee648cac482884673b9a946218bdbadef5d21cc07b113f73adefb520441c7488f51b5faf3581149e78" },
                { "fa", "a7785f243a42b58e6d2d71a877de146ad1c4921386954f1232edea92ea6158b6a5c7df767fd2bdfc4c989cb931b744e833feed2a86a164ccd28fe849e743ff7c" },
                { "ff", "4b96fedd806645c03af173937d0001020e874928e03bb184afe53aaac726e041d87a5e74111521de7f8e3123e12109e4d7653ea72ff7c2bb254e8c1fd509d245" },
                { "fi", "da2085346e5a0a81e9734bea8673d122d857dfe4ec2c7338d688dfeeef08a95c59f2160871a59ad20ff3875f9f6d3f55aae88fff0d6d584db0cd655a7ecb3065" },
                { "fr", "1a5f6663c4e90d112630409d9fc798670c1130a3c2c001c4f4744b326bf62c50bf39539d33ee22cff880a90957705cceb1c46383a57d95ebfdcd519dcffe0597" },
                { "fur", "32faf8f80f1a9638365aa7cb6d6b5317bcbd7370fb03674cab3434d62b0e5df2dbc3761e94c6a3f6ea642bdafead94bc443b1ae3691b54023a1575339678c66d" },
                { "fy-NL", "384ee832e49adafe697ca0d2f816e82f473a463e5647a06990dc526705545c5969ef2176d141a2f60e9d1b1d33fbe4752946b628658531a38c4ae9520de3fae5" },
                { "ga-IE", "d3baff67e70fca83ae514a6ba4d1ecba9d4542175b186d2d5cd3bcea239b4c016cd1dd4bdb1c4ae3e65a17266a241f183dbc854addeff822b9d3e3d89dcafa1c" },
                { "gd", "6c0ee6026d832ba3c8f3036a0515fb8054b8e78c7fc2fa7ebb32adec284d80ff0e416d6369990cecab41f864d844619985c54973779eed00aad6e32484e7e14e" },
                { "gl", "9f0388290f5019bb4c5d15a8e922a00f876068ab8aadf29e485a22c2b039d505c56a937be251c6cf645d616f974b91a1ee05ec87f4a8f3654d5f1ad8c0c68453" },
                { "gn", "7dd99f0cb6d6323197586ad143bc572192be146d0bd74f8dc86610199b6875bc88a904ec53812a365191a6f9f041185d5bcfa82db0b3c408ea438095c55a6732" },
                { "gu-IN", "bf76100f57de41a5acfa5016839532fb513389150f72fb2cd08b061e7fcd0b212aba970f38c8e01dc5b65e57f5d0adf229e3cf00e1b8624b33abf55afdd18ff9" },
                { "he", "7d4c97672fd6e91fc170f97e5e7e2bcd8973d24b98032aeaa24a78eb9cd1639c1bdc41d82d4bc3ca338b29a54e300ec1c8b70d8ee1d6482452d450f1deaa0d26" },
                { "hi-IN", "99916c5e35433fda35d1147f4d20e011bc4e1f8635b48dd690f95090c17aff7ed83d58aa3884635fa38044b3c711564716e5e3dbdff047779371bd95d2e30d30" },
                { "hr", "34b7c41915a12d4dc43fca0b03c53128829ebbd0879d867c4a9be3d876808112d50e1f432d52f2537c6a56329a0be9a43d2b08419d419d1d39c7fe9fa3e53239" },
                { "hsb", "2faada64a5ffad76c76f1a3e3226ec4b49952cfeeadcf483a959f3babea309c60f01cf6eb3cded2c4bd459632269af51b5644d10282dec02150fbfefe55cdbfd" },
                { "hu", "9b381065ac2862006cad3951a9c9f167a4a5ef3c8323a3afce0af73411d22dfc2c9a3fe6f6d9f30235a26f4e023bfb0cc3886f50877a2939b6fb79e730cb7c3b" },
                { "hy-AM", "1323fa321a4362c04a1981c5654ecb8a2254f8fdaabf69a45d1d9cfd1ff10e7c847afae29e54293e9577b08f59f470afb4bd2d0c605a9554a3a113446691bb51" },
                { "ia", "dcfe7b6e4ace83e5f535ef601998713e53ab489cb33bdf9e833aeb2411c3d911a2bef18ef5c94ca2dc813b104a9b64618a9c28edfca66bad58bb99b056a9362c" },
                { "id", "bf7bd6a7c1993160c7467835885c8947ab6d0496522fb88af34f4e071e9f9cc4bc46258870a08fb74251c2ad328cfec9423541d4dcd451c6caa9dc4e6d7ff80a" },
                { "is", "cc12f555b129e9d4617378ec83592551fea5eeae3edc8dfd619ad549548f128fd5f44428e33929c0aa984740821441a0d180ba0b95c62985fd38d98e4ef89fbb" },
                { "it", "93cfa2c4a20485c2af417a39293127be80556dd6a71e55e12638ea03596e64fa1de3e4f4ff77836da5c138c53578dc526701e4181cb7a8c2925aec4d3abe0ea2" },
                { "ja", "56f826797b50b34276b4d8180a4a0fd4a63915231bc986c847ed6e986cfd697b623091a9fdf22a6d1ca308b2af5458c0ab25c30e64f92d74aaf254a3e486cc2e" },
                { "ka", "a3b03dfd89cdcfbd70a405629e21c454dc9b4ce0b9f165f187ca8a057f5d70867cb048d1b65953b91ec5dd8c4ed866fdb4c530758d914e93167d85d6b62b6883" },
                { "kab", "3496294720e84209fca5ad8b109cbc74f3b903945b3a20cc440fa01e69456f7a3b6e8178ae1d118f1fcc6abb0a169881c10e6a00d5e183eeceab108a2449e1e5" },
                { "kk", "630ecbb40aa61bc0b519e24735b0199e10c922b0f8b743006a66facc2f3f19e911bd073d8d08b111d36d4d6e2a6733b1a310bbe0accee12a5d29b2d2266ee727" },
                { "km", "8b1db675ed9b45c3202467b60ac7b839c9f9673012fb2c30ed829d4f4a50fbe5a532ab9263d43bc011dcbbd9cb06b0e01243f759bfa19de9efd367ea6b3bb1df" },
                { "kn", "e3960ca5945434c9f9df118cbf180b54bfc67332eb7e4afeeb293357054bd883084a1db061b93007b3c6fa8fd179cae9e3f7fef776160f8725bdadeb3a8a01ae" },
                { "ko", "34ce98b48a3c02a35e22810ea7b84de90858dbc2e7d95bcd5dee6c4208a27c890ea04b7614d970fd383c7b58f3b3c649b5659c0a65edf8a27d835913c12aec7a" },
                { "lij", "18958b6d625402e9fbb4897c5e413f4d9297cd3efc17d5142aba99b90188dbde2e0bae05166695ab1936a80584bf783a39ec80a94a513dcfc875f59720a88cff" },
                { "lt", "8a9acb4082db68c61cfe7ee67744fac6d5e7cd862ee504e6bb37b9a554d6b73f7804eda5d9562b385f43866d347913605eb3076b4eb37d549327b8365b6f8c15" },
                { "lv", "2f6b0d399bfb7191f6cee75ffdcda30c7a19ce6964c72dd3100df12ee9928c9456196b4582281ee73fe675a93f7d79af60fbe667aa2f401e61ef5c5e63fe7ec9" },
                { "mk", "2fdb3de39eeace788d0180c236ad4eb15fb2304d9205d4b8c4d26001974ae405518b30e7f237757bcc36af49a2824341dec5b7a8adbb16b14c0088f5d59945c4" },
                { "mr", "8537604b475f0ad7ae92737d40f3e10d43ba4dcef309abec8c28c283b18fc08d9086e45cd6352072132ad6dd161623dbdf876e95561eba9a8224a986e5294cc2" },
                { "ms", "1ebc0bae6dfd2aaa84471faf3fdd57da18f3fcc26c4a3a709a21b6595d3b036e24460af02db87db64d10aa72de60f32956981514fae934065b82e1f8231f3628" },
                { "my", "d6fdc7d82f52ab1f9c32a48e1f3871b379b633a0c7ed3075a3d3af7d6dcf6ef95ee1c5bf7f2260c82121e5d463c7681a89c77a21b96ab2c09704eeb23a24c20c" },
                { "nb-NO", "46790ddf942ab4b9bf418a77270a96148726a1e6c94dfdb446ea2a79100d7cc251cf40f431614c2b4495124e81f077c380e953d3246858e2920014c07be92a16" },
                { "ne-NP", "011741d65e46bec258285ea43692b1fb121d725b8c668772545877bded1c2e23ed3cd121322bf9fe692c59a4285a822f9171542f659e2b7bd96405b94e82aaaa" },
                { "nl", "9d09e41922916d77c79cd626c0ec86b6122d7de7d3734c219c81e40b4619f9793ad44ce056ea6ae6dfd14e38376f62b94b82def8aea7d2e55a4d2015c049b04a" },
                { "nn-NO", "355dd4014394c62cbd241eb7b5dea8bc8c3dd17b9f8a856b59f2038b7033448834ec017afa07beaa3ddb794e9bb0e33ebc81cd66db08837f13974abfd97cb55b" },
                { "oc", "5b10eba5c8de53638d1374087cf38eecc5069b6ad655a4c00e3a953f1f15f10d887cf889078ed4d18661b4c896bb707fd762900493c1f66120bd261a72eddde2" },
                { "pa-IN", "4d53d89c67072d88511a11bf70c44578e0b94554c733af9694c7b0bf0d19e8b1aaf0f8077f745b17e4ba3fd2dc0bf68fa6fc891d9cff0503ab590477518393db" },
                { "pl", "a96df0acb974ca4c60abc1752bfc5026a734529c283320886e1f21f6da632d0a0e8de4de9e594aeabfa648c4b3256125cfbd26278c64c45b12ade4ce6ccf7ff2" },
                { "pt-BR", "f843f229631ffb42678a3b8079a081433faee9fb9ad82361dc3712571981030383943be48a1f81d44d5c61033ac9eb9781789bd9a251d9b4e28220bce3c1d2b8" },
                { "pt-PT", "5339bdf7ba89935ecaaf43346d9cdf2dc6e8581ec5135eb2c59832568b795ecadaaa74a8492365cc9d053ea3ddd2a3598ba3fed125eedf15096426885caa6fee" },
                { "rm", "d5d6dcd59b734b60a8f68b540a244c1d08934d514fb2be29afdf9fa11c95a67fd493384d84e55331fddd660147f7bebf84c8a1545bb2f549b9794a9d52fba4b7" },
                { "ro", "3394eacb2f7a3cdc2676a74d1246ae816395cd489648a0f0bb7335168a5deca04da6b6299e0096b385d69ee77fbcd085e0578051578f4d271951134e7420d63d" },
                { "ru", "d739149c2f7eaed2dac61dff8a90955a48c3857add54fcb512a1c0e47a026725a298b89fc1e68965bc80a84b0b17458f695fcd710b700dbfdf2f46deeed38c1a" },
                { "sat", "86f94807ac4d0af157a71a713d96cf9af3ccaa7b81597c5d9e669729559735b49442e0e551222d32ac79738f20798ba55553ad93acc5dc3004fdd9d22300ff04" },
                { "sc", "020fa06b52a3718b0fd2d9025ced664a8baedf24bef3c84c49e13deae2670728221cd1d0a57ad0a13262d257c508af6798031662f3b748a004d432e56c4b6180" },
                { "sco", "e550f12640773c514a4ed7d7d31881ef6102c7991c90a9ce11268fe003613363ca0327ef711d1fb93e31d53da8406a9a3599ef7dbcbbcdb4f9f2d9a37087a349" },
                { "si", "91f01757e8249a25fbc344820d4c597809ee160c89790ecd352efa3714c190e200135360750b4193e42b075e6f66e50ddac40ecd489a1fb33e6cae1d5baec45c" },
                { "sk", "28ef769177332dede10434346903a011a24f6dcfbe2c90f95eda597c505aadffac8c2c0d8e314a36f5bbe13ada74f190f6372cdd7a8da19390c8e52ff1429f66" },
                { "skr", "78ff4d795e41e149cb66238d2bc14b0550dc0f753dc2d2af0a41a3181a20697e9a8a6f8f3673dc5fc51591277da2946775715ba06a6c104da448d168ef89fb8c" },
                { "sl", "a1ed05b41ffab8f3a18781374cfc44f9c2a35b702c7ae0bfdaa6f340dea48e5dce69730aab31536fc1d5942e5c39f501f17efd0110744b5a97c2d8949609080b" },
                { "son", "a16460f0b8c9fabdd6709d58b39c174c325df3e6f8650991473e2de3a063dcd08c2e552d058ea28cb930ef0805c6aa379e0afa86bf75b6bacc2ee29515037ee6" },
                { "sq", "655d72de70f05cc9d30b6131687655532dc98a33509e7f78167da8d625fc2519dcc99ef51f74df079dea20d8f2e94992ba8aabd17f8a82acde18952566b4b291" },
                { "sr", "dea83423ebbba559edadeb940e8e4bac99209933ac4d1d5e58b653180491c85c99bd779210a9eb304ada4716a7b14c152c0044088a9b1158b9880063087f6837" },
                { "sv-SE", "fbce82bd7614ddd31d82b4dfdfa8de02feb8cc12d03e8bdeb0d6b16810775ffe2686fe727728d766676a4878db66df4dbcc7abea15f5d175dcb0f83cc3431a69" },
                { "szl", "57c1c6315697a484e75d28dfe5e5a41aa8976f00b5a669e3b4387de9f350d3131812265e2d8fa9fbab8e80f850a05f32ee561e32ab93177f16cd0c027ed0892c" },
                { "ta", "e869d945db982409a3379ef5ac394c868c580545f129529914845858c8ea77f3206640838bb2df038e31a04a72d24d4c88ddfc3170c0b4cd1ef3bdde68365adf" },
                { "te", "25476f7905800a125867110c31d6bb2febf848393f0e190e389c8f1f0dc8a240c26bed645a21fdd0a231f3aa81b8d1821d56aa1000a98c7e57fcc06e7ec587fb" },
                { "tg", "2860945c3d212cffdc8fd9646cd70870efab5de88fd9013db0932121d56f6f3556ac1001345cc2da48f157489c366c3d52f3b57ce4659d4a66b36eb8eeaeccac" },
                { "th", "669177e20936d3e1dc637fb9ad05472d249879e1dfae4f9af5d8a494b1b60f6f57b1c157e92670ad0aa896fe2e8290d67c77209e8f771acedbd70d03a753cbce" },
                { "tl", "2d29b0624661fe14fea90a55aa014e443c2ae608b7f1ad2b4b6dd353939ec83cef3a0bfb62d8f73582af332eccf474ecdaaee9492c1d499e8f12c93264bb5d42" },
                { "tr", "7fb40d7450be900970325987da95e902f3bae8765c8a7de7369ee5ae81189285d5521659cb7eac3ef84ab3030ad0f21d24cd590787e3f97802b5c4da8dbdb5ec" },
                { "trs", "b5bf298bf9bd21e9b0f8039478ef79bbad62a248a4fcdb9ccbcb56861464af7cf244f8930d42e31594b7221e0560cd1f47eb8da587ec8c040cd8ec96b4382424" },
                { "uk", "634d069d6b5f99f3e0b5834bea4687c1d629ed65a40440941b1176ca3b227b59e8413a8ae841f9ed22c2dd95b9e1f9fe4419c0e00bc9087aa3c9c7e88b0d70fe" },
                { "ur", "2af5f0f7929ab8e8ae7a41baeab8176b4ec4495fe0130fd0f34a33ad38936a7438e7574857545516423cfede0ce47ef8335eb5898cd9816f19796a98bee6e96c" },
                { "uz", "c91984129489ca5da87f6c2be52651e53236a83796df78b6eb590d314d20776e1db800775653a36ca201ba694e11040214abc3237838056278380bad4d0773e3" },
                { "vi", "aaa8678ea58b1347bb56172a146b13fb0115397b785debae8a78c38f15a0a4d1f69551d58d65a5d55cea385514c5537816fcaedc0ce8f891eef5b7fffb45f788" },
                { "xh", "f5b93a50d8a74ed7b62e41bebf0f7646ad5f43a6d95d5b588a8e3c0a4488f91464b7285af9f3cdccf596c03ce2f86403075ac2a77e8f2f6888d1823314dd2e6e" },
                { "zh-CN", "39bd18d6cf1addfedb3bfc6a05f19b44372e747513c559ab2c6b387570643c6ff7a4c3f95fac33e0f6319a502a7c53a63ea2bdd96c4a2e17c26c856c66a1ca10" },
                { "zh-TW", "ce46702e58f570000db262c06154511af1508f67811cef6d5e96c37b79bdb6017044f2d81e1e03ac1b742da96e8a4d00c2b7fd712b948514d5bfe7f3c3feb9cf" }
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
