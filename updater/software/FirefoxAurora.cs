﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023  Dirk Stolle

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
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "116.0b3";

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
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/116.0b3/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "0a3d7ce1fcf5e98de879ac3d3433984b1c7ab89649693e7d547951116adfea7e6f9ddc2ca868fc90c83c40bfd1f505e8ffa9babbfdd412fdd7d2a9e1e347253a" },
                { "af", "d209ebcf98d64c92723844d2dcb57d74adf4e873e34c6f701cee7a8552842c0e0c5976eaeb856aec4e354f3a00e3cba0dd36a0f0816bd142275ddae6de205d0a" },
                { "an", "4c38a69dbe5c9459da68ecbc5bad489165c8898161072db872c307c5a2b9cf26ca624fcc10cc37b29f99d3c79d76bfb9bd9cebd7b4c03a6798c38050598c8494" },
                { "ar", "07f6e0071955e87f4f6a63bd6025996b6d08685cb2acf7b7d098aefe14e83e7c397389252746ff7534c6938157913daf6ef808cff21b13dfd7643401e72c763d" },
                { "ast", "50a7b06d48709d0e5ce0920917f3ab13a36d53e7b4ced412e212072a500b3721c658a4411994250e2c192b731bebd0b959d69a29dc1280621ad5dc98bc0d4549" },
                { "az", "cffa52196c1081a3fa5d86fc13c04cd5906fd8c0cb96b8d90c5bc5ae4a1bbb7ce6fcabf5b11f205c23418c28e33c00ca9da472d19d846b12611ba41c883177fc" },
                { "be", "d48b2d08e4f9b49c60ea7b92ce8084c02456a63a019083ce7dd4df2cde0fb4615499de0beaa4f7e14e65aad062ddbdbbc0ea2a3fef6f6f668dba355ab35336dd" },
                { "bg", "4faadb31a5d10342a4793be471c39c9b71de0dfb11d8c498da1f4ea792bf2281207e2a3adc9ed007a29567ca63859769dab888dbd2261e3cbd78b415fa265222" },
                { "bn", "268fc821fb9efc52e469018cd3cd206eb5c0901649a3b61c07a8e74b67e21477073c6c472e342e5b9c98c5df1268afcf8f2988db8f46b66cf961d8e692a516e8" },
                { "br", "4debefb4d60f15160e9c68bf0bd6850fb1aa7ad0c6c3859b958b84de1ab048c24042134c1f2733666478a0fc4be106846fb6a486f9e659c4f9a2b6f32c0cc034" },
                { "bs", "6b6f42ba6c59639023f2c44b4370c4db6e0e2e054cf69b49b908fdd894e8f4ad35aa4a4e9277fa4eb860d24dc714f2844e5c59eb9e7e831067a860f47075204a" },
                { "ca", "d9c9bd319c1887604495bb2aaa287b995aa4734bf6c8e28adbdbc1f93cd216e108e83e88e9f88290303451a133544a998866ae13211017477004939a85cfdcaa" },
                { "cak", "ed5c644b6958de6c9858462670c4376e04cec3fe9e9acf0a24b83f1414a3f40ad8f661be139571f3489272dd2cc5e1fd490b620bee583ab9601562a4127aca05" },
                { "cs", "d48f853387221ea1729f390e781efde6ff69bd3081ebbefd70b22d092f850854bd52723d7f8948d9991eda1fb2af3b2a91f4bac6501f14a3b35e18c2345fafd3" },
                { "cy", "3b6a8f7c347ad1eb4941c4e8bef9de3a690fef1f915e245177c6be4aadeaa0900c909bd4901d3f6d687de2065d34ab1663416ea61f90fa34bf898e66ec23f5e1" },
                { "da", "8edbf09568120e13da5a752e02d2f687045ba0995ed0bc1850dd8ffaee6b544d029acb960ab4874485de0876a3002176c4d07b9abecee94ffafc2a992503ee6a" },
                { "de", "58e96951d60c623466a00efc2163663652e533ff638318cbab1ba12fdd424f7b8100f69644665997f1509da191f30eb833882f6e941afe5c9719d91a1d1040d2" },
                { "dsb", "627c9f118229c44703e68e1c6e7a2c9bbf1f9230da6442cc8454df6bf691d0277710796701ad35f1ece5d9d454048fbae7764f008322be0fa5a88695f4759068" },
                { "el", "fe6c503cbf02a064b363a404de08fdb97030bbe42e9d034cde236a9b993f2501e8d8d1db1538b3f417a71e19c4c64d94148a120a4b797f8a44d3475d8a246089" },
                { "en-CA", "f9019c3d5782bf0db188dd758be41b91586085d213c069435bb5978a5cfc20980fd223e4db8a77b7d08dc308750a6e877481afa4987f72b773081cb296904637" },
                { "en-GB", "9c080723e5664c1fc3ba1724edeb42a2d9dc48661fac6ec46a31ff4119a5aab5f3d6d7e9e0edd5a25b720e2a01c6b7440f088caa3656b925ada34ea9a609a17e" },
                { "en-US", "d75706ed8d3a743642329621f0d34539d05843f596e9865293c6de6906dd0ef6254ede257da56f941508bdc1fa36d92249a93f0c8c7468422b7ddd8180566491" },
                { "eo", "938d1fa4fa0ff08fcfd4800a299fe5c5640418caaf5674bc96303e1749d5324f59591373a9d4ca4310916e40ce8c5b68ab9176992d0f1596e8089bd21d730b2b" },
                { "es-AR", "a7e1b1e9f0a2482286e750c027bd78bd4eca33bdf9d322a1ee96386124ac8a3d6ff3471da78951fb42ecc76f771f2c01e7f4dd190a888da73612ec9bfb708541" },
                { "es-CL", "39f5093131792b58ac3bac280471a124078af18210f853575bc6e5eecc64648f4568edc902a26887a1273ac9c942a4b41d8f8ea9c65e976baf603a0a97edb693" },
                { "es-ES", "85710aa17bd4497504d3758190ab349fb9dddbc0ded3c03bacd2e21cec1d0216778c7e2ad13188aff9f873ec9ec51533c755f391283693155fe4995ef90bc9cd" },
                { "es-MX", "71a46907fbdc8f52e32c455bef42f0b0212eb097e59417b79999fd6f2eb42fe4beaa50593ab838a313b9ab515497d90cbf941ed74ac481bb529e9d56a27a76c7" },
                { "et", "92dd20136a272a0db61781146ac42486e2a9cfbb47004d2c62d9a95ca77e4765f0d42dd82791ae9c2200398bdbb073fa9dcf7cc12b2dad6fb80e38fdfeb14d8d" },
                { "eu", "b1d6b2a1fd11f5bde43a51d90baf041f3846188761ccf234a9b7bf429c23e2d8869c5f2c83332b1dca5114a34c185ac0f757ac460ea87e033b8ee1ae23ae2297" },
                { "fa", "be73d1dd9504b39e65bf0d4eb3bdab6751602f902abec1d1a87b0f56e803a477a55b00467a011dcca0ef020c6b4c6251a271288072234b8d15c280226846517d" },
                { "ff", "6a2e15bf8ddae09e161e5e18b6e3574a878a642716e63ae97672ae7e5a5cb0dcfdd89a38222ff155b4243302ddab1212542ecc19efa670581a6c4b1302c9d96f" },
                { "fi", "8ed4b977332e674b6101d626cefbc1de91bbd7c2a6b616ce84edc88bbcd73dff7df5c948fbcd25fa03b519dc9d2020ffa87faf03a14781e90e54533b426b7797" },
                { "fr", "c35e9bdd818deba002ec15679441dc2959c9063241be5c4bb19f924be75876f2524069d9e5df385b6df65e132d459280662ba0dbc6b5ef166d6085bea4833ded" },
                { "fur", "3f48157e5ad14e96db05a9480d6a3449b1a7b769ccde6c3df641a3519079a7d1dadf87098dac538d66cba035143a2c8a073ebc5aa6dee266923542b5f79b3238" },
                { "fy-NL", "6abf91fdb1cd606495b43fa488cf5c3aaa1bae34584ce748139f3530e475c1920dd13dbf4f4ba8925335931c66005179d73e6f1f8fa7a87302c4639473ac2810" },
                { "ga-IE", "02de621ed6502993a4d801a8914e68603d1b3416847d2d79cf407379ea1aec9ddcaaec1aa46582e49e83ffeca67fa5cc5215ad9393b32556a299d878669a57b1" },
                { "gd", "1fffcd68caed7db72736a423c947c8f72dc23cd78c90f484bc163fb69b11d7ed9e9f6825641f44824690031e45c39084e6d719858843824dcd3684b6f9187bcf" },
                { "gl", "5fcd3f1e07be474c264707f13f4851b014649585a6da407f362b1bcc7de025388f45f564a5cac23f9f262b0035857f34f996b25488ea8ce2ca142e7ccea43b2c" },
                { "gn", "5ccfdf5ec1a8fca2fd97f9094a202b11976409434616158338ceba7ac432c00c8928f4dc187171fe1a84546b5cf1a613cb0fa93907c782f529e6e02bb0834a9b" },
                { "gu-IN", "27104cb3ccef8cf4a40f633ea8fd6d1463ea98853d1cb8c769c4a76ec9b5414a0e6074e10a475d57cf705d1f73fb8ddef1334cfe209880a4f9bb03792d290818" },
                { "he", "3d4070aef36a9c3ff45cbc4266018b9e8f7162f45595d8434adcab087d904bd8b0b7205c91ccd3e3921c7ae6b660c142842814985b65cb01a56790355a7e89cc" },
                { "hi-IN", "e520bfaf43083a26dfef6122b7a813af75509333f077162bcd28b7e865421fdeed8bd2a660e0509f7099c766922b14781b54efcf480619a6a0a857c23f222674" },
                { "hr", "ede3ce8d9a2ead6fa5034d1fcf7ddc831a73b63f4112849ab81952210e9b488caa94b2813983a351f76856a6001809eadd5bab0ba2e757a9d9d5a132a8c5d9cd" },
                { "hsb", "bf5006f57be650d6c644386b7b4bd0a6506eb77aaa14282397137cb7ca5929e64826af61b1cb3ae9fe769516dcd9c59372933708267c765381a7edb66ca37810" },
                { "hu", "9e5b1729ae0c8666a4bbb00a4ab47d5f22eb2859362346c9266d4412f50f61b3114c89167c87bc206eb072cf505d0522e6da46d68440e27226ce76379e31c3eb" },
                { "hy-AM", "893d6ed9290e59165af4fd6c0fce05f62b606fc44edd6e7f9cd0feff39ae642eb6beb7fe027f71a7cb289fec94a3f9c694c44e2bc1addcb8ba6463f2cb0c01c8" },
                { "ia", "664c0520ffb94dd3b7ba676590149aec736ab14d50af112ff66bca62df08b21ff2db9b41db2a354e01f145f3289b1bd5c23a85d3cd2b527d8bf9396519c40489" },
                { "id", "0321292d5ab2b842076bb32aa1140268733b9698264d7dc17b327805e0d014b25209bc3fb9290622c4003765610029dc90056b5ac5aad12b1054d28d0df5d52c" },
                { "is", "89cef393d49bf6a49dea95fba27e4b273af59fd0f4fda5ea899e682af079bcfbacb340f8f3de41a0fb4f9537ca59b213d6f0a6b9c48ebeb41242e9389320e3a9" },
                { "it", "b4b9dbf1aa6594c360b395e0cdaf6d44f37fa0ad11f28636e874587245da4170e98942566cbda8019d5e2b342136767c517bb6cb5beb6699b4cabc619c3ceb65" },
                { "ja", "e5623ab1ea02f1555d71da0298a46d611d16709310805204679a2bb23a4b02b2154865725941b79515d85e7074bd5bfd1f7676fa765d1e463ef474f5acedc324" },
                { "ka", "9954195271e6d0304c93d1d371fcab8327d20a52800ca9c4f13057b6a21c049fefdf01d46e55a7db3acab2b7c6aa21f827f32699b0383ca918fc7d9f1f090c1e" },
                { "kab", "c330a0628c486f196724f162327bb49cdf92c11ca4dbd428f7f7593ecf180eb210984242f45a10858d7f0648d55ba8b8b23a99633886298acc7f597a035ad781" },
                { "kk", "e2d45ccf9d0385d650c487b4a8f48a156d2a7e5bb65ea9ef20ead0988ec8e59ebc121a5b483c6fbd281197d5ac12c704a5cc2a218d932d9d9328766dc81d74e1" },
                { "km", "06d4598b23d72f3c4d5c3c71e0bcbb681dfaf57df8b23be6180d5aa8932700082f0bd4f9b1e6d0c6bed33172ecf7eb68c91eef4d51114654b5c3c44ccb52620d" },
                { "kn", "05d7669fc853766dbfa4f8c2c1a3ca81ef01032ed86cb84611b0830b88a6a9bf00ccd58d460bf6fa97d2874e2512cbb20b834984818461cb1a1925ebdbb9e77c" },
                { "ko", "173f5bf0e9ef63ba80324539e02d8784b4de968a5320c4276199073080dcdcd8a50f6c9981d34620e97918242d0e3a5d174030492adbfad2c01136ba342f8ded" },
                { "lij", "2dc989dcf0766403a9032d544b9316714732d027723fa882094f3c1e27016ff708470096d2e58a809cdc88d3b24a0a79bb218142003bcb4b21b10c4514468c3a" },
                { "lt", "e4672153a251a5e6a91607229264420841a0ed1582a94371ec78f7f6243c7c1d4befcc63ec4a2f6a02c34b72a3174a712bfd2433926b4a993e21383913316bff" },
                { "lv", "618f388e41b158c1b82496cf860d4631b679ad5cb3ddbc3cea42ecfa1eecff27f31938d0a5359362be51a722bb6c734eccac7104ad8a4cad600ade1ea3c56a9a" },
                { "mk", "1a43fd81f364c6a6359093c8c733b81c789015a31209d2b527fa46d7dca933c87f13e9fa53606b6ed703de35b564a951873a17a68d979f8c19f21bba1fd0e486" },
                { "mr", "6d5789ade9c877a6c43cbd3f536d60ac3269ae6645b8a47ca3ab60cceeb5bebc8c23cec9f960cf49a815afb6eec9455a2a6467a82c80405c34b03c1b6ef318f6" },
                { "ms", "a862e4bb926719f7c0c584bfd1fb76ef0e2486d5d487476993cf061b21fc18ee57ffcb090c3442ce698c52fd1ac288dc8c8e1092551004ed34058f5b8bee9c43" },
                { "my", "a265afe1317546f7be4acaf556fa6c75688d2ff15336915bf3782d2794cbfa24a85fd73b564b1eb2d34e4dbaf9273345e1c5c09307ece6504c20ef7ff0234cf6" },
                { "nb-NO", "181496cc5d3ee1b59e48bc7fd954ab8cc1706833fdac77fc41534632c0f71885af57b304b5be397d9bd46d4b72ba1eaf1301f8c08ecb91bcdc8a387a788e97cb" },
                { "ne-NP", "416a56365ad511932fc74b92865d20b55474adf27cf5c9cd68ebafefac0e968dc266b61a22a6f2dd48baea7bd5155db73111172220cfc8af547e21cda452a208" },
                { "nl", "567431bba5ce76d3e6f29d8d7825264988406b76d704e96caed851413733be7f9526e0c1acb672e58ee23fc32c8c0f2c773ec70f88ffac4355c66f0958620b81" },
                { "nn-NO", "c6a252e8249b76d6f0a3daa364117a6b0c38174c602231f298bc647f164731f86b33adeafc7a034de125f02cc962b323329cf830ae3ecd765f211679b52b2c11" },
                { "oc", "f4c4da554be3ef1e44403db466fe9d3bd5a2bff962546986c0fc8034f9301d86cfe30f0621b71cd380ea260268b499e659d050251773dfdd80831f2a52530471" },
                { "pa-IN", "f89d8afbff9733e432cee13c0398cb1a9aca2246eaf67d497212f7e0be77f38bd572bdcf972852e4f85d38345f304ad18c80ee78071dd199f850b4b24b903074" },
                { "pl", "ed7ac8b43f2de85bea99ad2ff455412349c600da3568c4e11d613c6d3c811f84b64f377cb78c20ee6e59916ce33ec34ce873d6f8cd5a07192710989111dd8cbf" },
                { "pt-BR", "baff75684de1f59b6cdaa64a49de248289ea14cfc1dc2d79131ad5b3768c8643393ea1d2584835e0ec89211fb01c53eef83ca6ab80ecdcd947fc026d19763c86" },
                { "pt-PT", "765753ae5ae3c7b8a167962754859a29b7184454d5ce96cd06a8285bde5c61716beb3bdd98b4498d03e15f078d291109515aa23477e9996073dc180bcfac9665" },
                { "rm", "b908eef3e609ce0150a2c99bed8aaa28dcda2f7ab6d8f7c0cd6668675c9aa1b87b2914f4845e6a413c8f20180774cb6efc9e595a7e0a717b3737cd128f00811f" },
                { "ro", "8c1203198ed104cdef2c9a569751f8d18ee5f252b7b5970bf8b01e25c6d13ae629e6a6088f84bcff0d5f9dcf761153ac3792a94549924751c82434b1c1a2ed50" },
                { "ru", "c79499f729dc7b492232bf06183372965053b56d23f4316a7a0272ecd38025410b2141395a295ac31cb9b559d8e6f6f19e9a90f851abcbf9b567067ea66db866" },
                { "sc", "efe04438c8bc14b06ddd3e71ea8804de0ae00d62406606e90d5a3fc7809e75d9bfce377e994b61aef22253f0ffdd04e1b777243b1d92e171abccca080148fed0" },
                { "sco", "eb1db82546059ba464a60c3095b403d23f4b55d1685fc0b1eebe07f07a4a7d90024a51796df089badb488c989d3c9b4a1c35fa20bf8e69417b45c15e8d227201" },
                { "si", "11533db1f61664fc9dbc1bfd1b0ddba92b0fae1621a50660cbda64476a73dbea9b89e35d2f59206eea21e16138e171d2be01b7b4d3f146b44cea92396d958930" },
                { "sk", "0c79efd0aadd51fec587d1d6ed62dcd9c39d58f69a931abda26f1f2ff81af256f54b1a2994d0372998b0d8f5423a8b56694e53fd5437c145dd2d082c64f15ccf" },
                { "sl", "88acf5fce6bd9ea19a0dae206f4469333fd876108ac80e468fe8485e253a6cb218c89beecff3d89380a4a3e0ae38afa16cfd9c85ff935843334f4e933a992723" },
                { "son", "f9e04942415b63b40268d1a601f1470cf66a2532cb1b94c4fa0aa3744cf6df67cefa233dbd3129305dbd6cdf6def66cad03364e9889fa6a608898f85acd4eb18" },
                { "sq", "02b09e9d7389fdae249f8823a6dd45af61d05d227067f4094f82a118b8577509226c7a36a43d0410fce777288932ffb8e20e365add369fb37939f7a8abb14e1a" },
                { "sr", "43c1d5c68a890dd3c6ea31623b8672ca5f4afdeb8bb69ae12ecf72cb8dcf740d9492777b434ef639efc9466e72a07314b684b40d4d4b4fa52a0b32a18470f53b" },
                { "sv-SE", "85838327f776ceab509cfa32f64bb99c7a6361454b3b04adc9b445ac544c3664c8a2a1f8e236d595465b89431d7b898f21f1b4898ff2c683dca6360f6fdff28f" },
                { "szl", "275e2fd6f08b9e87cdbaad1472de6965155f6886dd6ee1f449315ed9f0a125ce339c9ecc34ee24ed50e47a76ebb1cfd423609a81ac96af90541f72a10a09ef33" },
                { "ta", "fde3ad9e5f5e1bfc3a5785c07eedd1609af554aeff1f0482387fe22c893ccb9e446fb01ddba3874aa5cc36aa4f5b6f8a8d3ddb09e3f6b9f99c23eaae6f9dcfe8" },
                { "te", "78d6e5d4298f87d211740d2df20650d14283860fee08a02d2432266dc09793cbe1e4ab9ca658a60ac90d3d9fd8eae6c74070595a34244a0627744e6d667886de" },
                { "tg", "f6798a590cd67968bf40babcdac73152b377801a47a5f25ddde1def61b91a97bf5852650766a0a7c94fab80994a2fed29848a8e6b06fe0329cb94a0d780664e1" },
                { "th", "2f017117ecc0573d71a31b277b4ed021534e8b9e4ac1c899d2ba42263b7e807ce3269612ef96acb715f706a106c9d81eb5a654e2fceb7d05f85eee3f04c837f7" },
                { "tl", "2432c7a74bb1a2b689065651612571c1b021e1cd8ece3ffd961ac43f75612fc577a2fdb981ca0b06c49cd5b8ce86d79e3a992bfc20e91b2bf12d021ed1514ec1" },
                { "tr", "e0b388b2c47b8715fdf69fc45ad305132e6cb9229779605d741abc1b07a2e17de2b0e4763baccba7b618ae36d00c8d79f20c8480c1c9919fb1b8a3ff505b13ec" },
                { "trs", "42d0071443790bd81bbd4640269e2465819f8b267a38e0b21d9fb06fcf8ab5d6b48b81e0bfc642d67c0d05784e762bdc30c3389587d0edc6c9b6d906d85da1b1" },
                { "uk", "51542e41cd8cabd2c5f714401ec74be3ab0b761b5be2356fe9e46e18b3008d2c4be7a036d0ed27cd4b7af09291e27dd9803524fa38349da135bd84fd946290f8" },
                { "ur", "6c017d1443acd5c939e74261b2a146cb2385900286e224cab64f753ff127e8774910583c0649d61d0ad962849f5047869fbc304299063c328cdc0aba71650224" },
                { "uz", "c083b3fe55e85e2bfedfd036b278088f96f6a7aa6e521b52fdfb48e411f71dcbcc7fbfcfb915108ffe2338780131510f61f7889c154ce4435d2f4b4c5e44c6c0" },
                { "vi", "6042f52805e2ebd5eaa172cb0b77dcfffa8eb2399a8e4377e564fd02af531e9741f9939d3a52be5b70601e069a9316729a04accf5d35a5a4b3545dd8eff3ee23" },
                { "xh", "cab8a82e20cfb3fe9f7321d374f69a426ff9d5bcc4f880aec27b13ab61737735dd4ddc5b7d1d9e52fde5b08c06c24a4e0fd1ba1d1ee26bb0271d97c693ecdbcd" },
                { "zh-CN", "8c9dba0e45934394b45065d0e1087fbb2135a5ddf690c347ada78ce4024fc1dac864375752af7895730e996d5b7da038356a423db7248ac6d974ecb134b7920f" },
                { "zh-TW", "ffe43c48d0760196a34a7fe5c448a20116bd4894489caf34e3c61435e2404b0a1d1b4c004fda1f06c99ee9404a94f4b30305556c84035c59befde4b341743fe9" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/116.0b3/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "0831371d4652e28920d4d0f55f45ea1b61a31052a9f162030377c7356f57c2c8408832854629766552dce5c825b7d23e848402ab86560c886627fe361f616676" },
                { "af", "f2746c337da140334c91b3255cc61c6be5b260eefd2a4bbfd253801e002aab185f871e0afe58303cd67bf8e49492991595784386103a49dfb3e6f87c5c5228c8" },
                { "an", "ac40a6215b073cd97529574354b449dd43627475db83eababce6dec19ceb10b381aca6c4d2a69c8f188d745cb5df38b32fca2062c730bf703f2999d85db9fe79" },
                { "ar", "6c93e14fd62e85e541e14d7a5d7a3ba02b644174a752a34ae93120342304c592f77c85480b2a2f405244d65438033a37f518d7d678ce53fe7812f2853bce730f" },
                { "ast", "4da8fc555cb034a34025514b9a849f14ed4002130b5a077313a70b9deec37c5f7e0994978297c30e6586f4c7c71aee93af7395fabada93fa6165a61de81031eb" },
                { "az", "4831a6359c5c3ebbf69b938fc93ed286725bdb21e83f5f1f967182fa62685b1f72e8f21698b413362eab131abe18d09299385f0e5998b705fbb7fdca9cd24c6d" },
                { "be", "a07ad7066781566ccb2e1c86a547e320dc9bd387930a70eef981a80f8dc037c6458bfc8959d13af27aca101229ba1edfc570a6b5d6c3f0a020c5350e4ef3bb73" },
                { "bg", "6751fda86a3da7f26bf9018488f492ec57802d67326acea2d23500fc936d3dbc228d033d03bd33ee3ba020371d15b07611ccd047b3f9a430aae793f7493c14f6" },
                { "bn", "8baf48b75ce67e7853e82096c648db936c85eeb0619582edf88e2bc0a1e829cdc2f6c704ce7e6185fb10bf4a82cd1927570dc33c3a59fe86e2b21a3a3098cfdf" },
                { "br", "25aaf5322893dc3085e00c41ca910c8d49cb4fbe77874118c1e3638ce8cb929f250073c1e6179656b4b7eeb0595159ac7b80704f9f604ada0bc18fd38ebb27ec" },
                { "bs", "cf2fe27b407b1855f373051362c0ee361c90984b882d855fca67ebc392c0383a849327f961c745be0937737c19df4003ad52952e62d9aeff6e99eb458857ecca" },
                { "ca", "606ec86c15a43a517f70ec0219e3585d0eb4584f5d9a77c1528bd93a84e7ac2e0ddd5dda31f03924288199c2331bd5d785f76b637359abb19debb5a254f3d068" },
                { "cak", "e2baee5371df2204f7287581bb2d8a1bcd53a71ce39e4c480c3c8fb68b6d7934079c7e46081986a7812791104677c7e0b0f0b5cd81ddcf33efa459bff9b76f8d" },
                { "cs", "a392862fa223aaca6404d29152724709394ef9e93719751b84b76aff083b74827f3504b85d5497222328cffa7dab8ea684923caf295ad237fd9af6adc808902c" },
                { "cy", "c376ac7cb1f48adec7069398ce8ee296d73526f576dc20d5bf6772b06b006f28e7aa32d08f1ff65020a0f0e658ee760ffaccd339e68bf63a2e439a70c8f8df50" },
                { "da", "23008a13f274ce8b01134b76fc21fda9a39c6a79955be4f1dfef914d67929d2af91e399da8c8369246692d16d046eea6d289d1572044825dbd357a58a627b172" },
                { "de", "5ab406129db7e4bc2245df2148a32f82db5454894b55d57e939db92cbc0715712b7a4364aa83bd7da9537e46456399f7152ecbeb28d5bb419b8769c754a03891" },
                { "dsb", "c3b913809d2632195b27a20c23cef967ec02829563d92b81931dc0117bb3c48a25092d9a584a812c1124b03358c8d26c43de462c33cb3c6f64e16f15d0bfae44" },
                { "el", "bd6c6cbdead5bfa414f8b77c7399de00ef2ec33a6b2c607d8f542241ba6c73d234810f6af83af3d82891917a3ab777410bda6784b44a2104c41be334d10df6fe" },
                { "en-CA", "753113acf0ed1c32328af1b3fe13d20b07d175038114ed0ea71e608f3e363ad2fe9b0a09d51d29634865858a4c6fe2828b08ab0ce0abd9be9d035618caa01adb" },
                { "en-GB", "230221091ef7ecd9c9f7746b6543b2d016e8143a656d497a9f30fcbd794297d69dfc7a9825483845a03e6c3d4b753b026daf156804444d370dee144e02e1765c" },
                { "en-US", "b5e26275740c8c8dfc748f09ffbfda93bdebf2f84ef5d9a17064ebc9a7152dd44f689fe6e700aa00f7348ae3b5eec2bd56746ee151e382f67a3d7229f16480e6" },
                { "eo", "9e6b787aee51a5e26a067c6f09667d894934a2be51a91d38a22770009d11b015c204eb565c0576b5b199cf90ecf672cd4661b706caad482dd5a72649b03cafec" },
                { "es-AR", "3c1267e86fd3d376bc82edab542f00c464861204fb55f0de71d5e9c8cc0f84fda1a144df262ba9deab949cf772569ed6e837a579336e339ef60a11bdcde33a7f" },
                { "es-CL", "768a0cf1487bf9881a346b63f4f287bdb54d7641199ab35873374470585dabbea4684f131cb823368af00c601ee5fc6b846bcaa50213d348756a1f1eda97bc45" },
                { "es-ES", "f17e545c70893b0d17a9bd9c5c2174c185d5b0ad7e859a136de3d1dac4169c4f4c601ec0b53b51fdf76fce545b91e0d86390e6ad277197f44989f38a7fe80688" },
                { "es-MX", "bf6016f5f2684802374bbf209bfa13d66fa542a222f696d9c21d21e5df5e46f9adc06b32bd955d284ee10bcf1608872e30e7a898cd9168a0cbb5ce20c59af105" },
                { "et", "2c276d50fc934c5a9110548089d41941c42d3a140abf653fb2f8eb21cabf894cd40e16579296abae959b3af1d0bfdcfc9263e19d959c796a107271fe6940382e" },
                { "eu", "22ef7a6454aa9f4ca562e0f5cc2066c2ca112f0af9bf284707340a3e810ad5562d32b6888dd8b71447497a015826260cf8d18349e139c93a78221361c0c1cf4f" },
                { "fa", "14e8d669324d0c159d5dbd379cc208c4e38796821f10ee127332ffc60bab760015ee78c0db0d0dd2b5a0a004f0ada79a816edec7ef8f2bf5b147b718681763e5" },
                { "ff", "0994798b84dec03c052e3f49f0b61698b46510dfc67c228893b1c5fc2bcd9d41ec5d06e18850b2a13a8173fbf1f227c3bfd1b1e06111591568b67811f3ee1c48" },
                { "fi", "11872e2ffece67a3e1ed04f117591136fb5406324ad0068291bbcce2c4f8cb0321a53f904f78411ac8e6d7ef5dfce3ab0168376a52229818dfb75b2799075f0b" },
                { "fr", "fd6efbac6c1c1c625635e517b43a9345cf0f32159f2011dbdcf091ebb3a7d6c5b09078036e8ff54a9ccc933b9a3560b728b8ca2f29201977c30bab9002becb58" },
                { "fur", "2359e1da6757752d2469a9e375bd4eedf894272890f42ea36469f36be77f7fc8bbfa6a7d0a9dd0c8432aa79e01f2040b755fbfd7fdaebd094e4e509cb3f0fb3b" },
                { "fy-NL", "d4046101d77715ab34e5843a00bc7928f975b910f3c654895a46d099e1ba009705e8ff963252b2abd33bceaccb9797ad68b32013c3d90df01757ae6103eb61ad" },
                { "ga-IE", "d364fb1fb26a89a6081726cf08ddd45d48482135cfdcabf712e808c4bd7eb2d8536c473142fee8029102c3f82b3192f15ab1d89daccf15553cbee092bcb7d2f4" },
                { "gd", "71e324b54dd38b5448ca99609023143a73859236e31cb13dfd0f61d76c418b80c5bda5e0fe89dd366fc73a53db54a2f2ec6b497a507ca475c98182d47830f37d" },
                { "gl", "0a2d74ef85e53ad173e64a5d48df5c1c4845503f3c72e116df2fdc01d2b99e9328b965f3b38591a35ab869fca0afb104b594ad1414eecccd641eb52267c511ce" },
                { "gn", "2cb3868d0015ddf7581b17e82f316ddec00ad611eb254516e6b9db7e65db8b5bece1d62b9c279b18d5d2d8c9076f116b036429767dc2d14a3196a01dff66715d" },
                { "gu-IN", "fae0a234b5db73dfcd9262acdb42a6a0257987cf1b390199ac866a368ebad6832f0ff636d54ae7f89611eabcaa49dc78b7a879c779282988c569b480d3b10de4" },
                { "he", "f39be2109731f9b2d6ebc0d58dcd82dbe074da212a2ae983cf37fbace489bf5e53692683ddef59054cb79f1612405e2b6fb85f0500fecc29023f3bd664355e5b" },
                { "hi-IN", "fd620268acaac738bc8b22a1778716cd5da79405c2e3c71511e1590316f48cb08d6ba9edfed54f73a605ff12d11d7c1f6216d929d6b18d72d8fcd5690f7b99d7" },
                { "hr", "1fb92c9922fc244c33ec599388d7763da07d29e21ef8dd02502cd1b2a2e2383956a19069b8d494d19fe45ffe6632ed04bd0a454949d969b6069d23050735681f" },
                { "hsb", "f083c1ba0d75a750eef2a1df018a9d6714a463f0bd9405aa92b4d0250058d07d880b7d7721fc95f07840160bd437af3f42202ef5840a7731f855674314c037f0" },
                { "hu", "73c25f5fcf292b63c7c2b5b975668528fdf826dc48bcd765507f0b4e636e1c5568deb6c0708fa7efc5ab5d6e9e2fba27986f0b2e58d3ca23a06f1af38cf7187a" },
                { "hy-AM", "cb8f3ba2c68e0b16c4d7e4acf70cc8d917afcfaa5e80efedfa1e609bd3de539e2daccc8a251e4f0eb8c8a8234b3deee6ed3727a4dd3f293cd4344049aec6eba1" },
                { "ia", "7f0b306c3ad35dd70ec53802025bcfbddd79cc2f525795e91352c73824097a22c1fd2a0e7d0b87b5d7ab7e75bce493792c61679b4107de399b973b14850357f2" },
                { "id", "706e64de124327033c63b868be3b219d75aa19d6311cc03e8d5cd456b8d4facc0d6e7916833d02b828b053439921a0e633c34a5316c6bbcf5e284dd795049b24" },
                { "is", "a4b6df729fe55433b9b9ccc8eac9513b9aed570bf3ceda9057d65b5fb675775e653a414746f91701c3718029a48919f4987a650a1fdd0501d89306d601e97f9d" },
                { "it", "13274abd4f064e730262b7a4b2d4cd3094cc6906cde6addfac3b266f738d14cfaaa2e835cb0ad5200e353a7d1c2868774b05943da787c9d1fbc84b5defeb1816" },
                { "ja", "8ed8e8742d9b171073b63edbcec0165fa8834216412e8e6b7217fd2ff81ee9b988f83ccb1078abbd78f0a48e19187f354451a6d2ba1bd8edf57d4a6b15a493c6" },
                { "ka", "e420b3e46661f2ca38bb8c76de7edfdce82faa65ca40e7614c8c54da0a260372c7a15b34ffc2a90aa861c597aa069c0ade374fc1f462460b26932db8637ea0d9" },
                { "kab", "85a83e25ba138048490bcadd3542c3c5b6343128a24cc664c81e94f3485b14cb35d500e9e55c051ce6ed8bc330913b2945a3d6f6a15dcfb5b3a2465ac03460b4" },
                { "kk", "f2193357dc389f2aa1d646226a4a40083615626fda37d5e59910dd56db4344fcfd7505ac55c21fbf12570abb18fd85b423777566cf28778e7525b7d874beb118" },
                { "km", "81882bcb10bf6d982fd59c5a36cb6d66e358f2c80dcb4765a8d5efce9092be6cfdbe1889c5165b04b710fcd20140e979c3bfc8725d367290c80c54ec6ef440ac" },
                { "kn", "96077942a1bf94fc49ab9bc1b4152ee189d22fc1cc70f5e5cf8a9727e215f68bfaf06e204515f6496ddd8bd5cca87fdb7d098a7b49217627d8167aeba812c0dc" },
                { "ko", "af433c1207a22d8bcdd7553e9ec02e365838184997de2bd2638bebe0336270aaf7c1b7a52bebdbe38da67cbb599b6425aa8215e981000f184da68ed348803a61" },
                { "lij", "879c31c8fe414e891ba15334b5851a241fbd6e870ccb4e511fd185662d77087b4c09809f51987e2a751543bf30ed2e95c82899f0afd9a1e69cbcd8e19ad78c3c" },
                { "lt", "61ee09d431696114dc6844584307ea26f10f364f701beb97d0fb0f5003929281301da555a35e0a9d6ba5c751efd044c825764924632f743475757b843155859a" },
                { "lv", "9954bede5224d76df81e2988c2772b6e6e42faad8e35647eeb076491d7da33cbd59e2942a74fb3b6c25262da16c62904497a7a3a7eecb4c61486e7520c0d2a67" },
                { "mk", "755fe667224c3b9ae23084c0ecc3fcf6422b81f8500188381b40d20a5b1dd6f6509b63b280f8190cdc11b48499935f72ee48ccf3866e02b601ad2ac8d4ce2755" },
                { "mr", "e1e62937dfd9fbeb3c7a3ca99b21d706182d326887a9eeb0459152127a0ac9fac9398864ff0e6094f242facb204c6bf7e41e3eb74bde16d51e0a325748426939" },
                { "ms", "081d6a7a59c58bc9d85dade53d14e6de55ed8493bd9688ff446e79b3f730fe9b7918e1230246cb7df5f3950b3d22caf039513ad1fdadaf5dffa059163fc3a924" },
                { "my", "bff641aef26283e115e5dc6f36c1f2c591a713fd41d73cc82370e516f538a9db56618a06a18180da9f7796b3e1e008897166e4a376651d3d83691c7099f4297f" },
                { "nb-NO", "eae4fe1bbe5bded8afc996c9f0fde1cd28a3334123d8caef2908ada3ce027b5bc1e6522c26ec598083028e0eacb07ced31bb0eb7a82b244b5d80b432ef1c4ef1" },
                { "ne-NP", "9a11a2dafea98dd6feca8992610cd7dac447a59fc097592f67bc3062cada9f3253245f053c537a196dd90f5a7d13a3892674e9d45a150e2d9b9991fd8b93e619" },
                { "nl", "469e4652a5600496132935d9009d63882ced719d118e7e8f34e533478ccb4026ccc82528ea7eadddb69817c3a1a33783064b4f9da7bced50738af51a10b982fb" },
                { "nn-NO", "18c4b77f3b78c71e90798317e96d55e1aa6297a777d77e06a9a2b7a360bac322cdd53477319aa3b0b1668ac30953b6d6a1bcbe3d502c4b55f41cfc08c473cb79" },
                { "oc", "04bd26bf2b4b2d45a07c86438e00b110d3db36c47b7477a706f88509d4dfc13fe61439520dc9b1feb33d8148f1027a5e5753b8e32b6500faae14fcfbf55fdf51" },
                { "pa-IN", "c55737a787480c83ceec2fc9eedf3cc96ddd7e691d806c5b461e5f44d271f5954db77741ce9d2de09ef6c4472348b72f6ae1cc18d14e1690a41da3663083ef65" },
                { "pl", "2c9deda425bcb29d1c9b9735f711b54f19e4cdd09fad2e2e7dd76961b5c93c9bbf17cb4160befccdccba436f0d69d717c70898fb543d3711979059ccc4f0b34d" },
                { "pt-BR", "9af99aabe9a4ed940ec5dd36805317441980656dc2e69f5c54f934721a39343cbfd292a0fd6975e3fd8ed88ecb099b0b1909a986e1faef4a91ff41951e45f4a5" },
                { "pt-PT", "b836ce2937bbbd3f941ee2af5e56d9def071009cebc42db0022e5afb49925a2b3b8447c8002f680f2202b9fcc5f95bd94211c8a1d3ae928f441fcff9d43a678a" },
                { "rm", "0ed9c5eb8f4359015efaa89d9f5f1877e497a4c14d55cb10a3afc25342d538f69a9a8408f9678d1a2caef43a79f6166f461bde255d760bc906660fbc3efca97d" },
                { "ro", "2afa59f85f49f0387bbdb3df6bd25fb12f137882fb0a451bae759c00a39093ebead018a54257a73751d2350657b78428408365682edef001932a6b85d09303aa" },
                { "ru", "f67aa61df4b3e21ef9c5ed9f5856568c337cdb272d1fd992161aa1908414f57628aa2ff96abb0ee3c768e788cb16e99eccb2918f7b14df85b58438a92ead8605" },
                { "sc", "7fc46dd71a1c6489f2cea8f4a9623a37b915c99b302ba5149bc340efdd98fe71098b761f9b16bf9aae808443f616e93096f06bfa4654420c748cd568cdae0d88" },
                { "sco", "9efead3ed3a94ee6ffe6f24da27a275f7f3e1bec65c6eed784dc7983e4bbeebd80c10dc37fd25a9edc848c59348148032a59a9896b4a65c05a5f5b6c608f2fc7" },
                { "si", "d8dd79b55786547a0a0614cede57cf397f1688ccda434c09e6448d2e3dd43dc78f3042753e383e11decc3a1f77fba40ddbc53808190801039eb911b569c00960" },
                { "sk", "b4385718e2bd4767a7758d44e1ea865fb25e6228aae27db2b951778c78d3709d56e47c60b43a83ad384ccf2c490e310781ce23c8ab6b233c250b1efef76a21bd" },
                { "sl", "b988e6fcbde64466ff1e22bece01ef478b7898e4f8e1c90452aba5ff251e0e923c84c5cc60d8798c8cff80ffb3e1e0e10504be19e08e27eec342a2a779faceca" },
                { "son", "0ba2ef130b3e879a90e28a4d6a30d2be135476372149e529bc9ec4c0274a19bdc467aae323bac98b710a6dd2fd64ae259f16c756561ba65be94886880b645818" },
                { "sq", "d04597088cebbc3422dda76bc9c516b3d1bf7a59851f060192b882bf8cf435bf6b729c54470dbbcb0ced78f5fd1c64e280ab3f7749a91dd9540fadd339c7da5d" },
                { "sr", "1cc6c317f582d43527b645addb7838eac41deb2a54871ed8f77099bd3306d5f47bf1c2b9b587d8ae04ddac1b6fdee294156445784b36c4731708a15d4aba6c32" },
                { "sv-SE", "4692ab0f282d003c066bde37d954677317263abe0165e9c1d91adedb5d59c789bfdaabd3b650295120562c8c446d2762e1deff546a8ed6f9cd8673031db210fe" },
                { "szl", "594f0e3453bcfe594e3d8717d7ab589e2c5d150cb211f188d7be495f716fae00734925f59155129c82c53a3135f2ffe74dc7d413c103ebbfc8a5e5f018091b6e" },
                { "ta", "11e3a463eeaf2c3e84d6d1a5332a4c490e3208b0607963bd179ab2a2192402769117ca43ce2e5d690463d1e349c0428d7e15fb2873ec0dd72ce73f62a57d0c68" },
                { "te", "800e56bbd10b8476bfa7a3d0a9c76e10edc5be7f7ccfe5c9d96a65ea22cba1bba315933911e281bbef60037f7fcc34317c1ac1b057dab6808dda8a1f9cc62d9b" },
                { "tg", "107af8606faac37e7d55a9404063ac608a23c65d1e99ee80765f13c9c45737b2b7941a4dea5cee3c787f5cc41cfd95132cd9ca71ddeb2529b4682ad634b56493" },
                { "th", "8da93c06524398646a18cecf3b12e7d115c60a88276dd1f72b50e33b485b974984203228d133c132e479876d67c136ab65a763a2c4885d26f29c9634f5fa11af" },
                { "tl", "28df40ba10461d5f8e0a1e24e6afe696bf804858a42a5ad3babf4565395975e826b27e8d03735937561f26da6635aa1bdd72db2ff783b47d6d2e7ac3c52cb3f9" },
                { "tr", "e0c4a751093c82ed7452a5d13a75f061dc21afc4406b0728e2e32767bdbdb5051d9e2d8fd18e302b965ff95c75a4d00e00c61299d548719e43d02cf7ea9def19" },
                { "trs", "47630626b28c407be47fcc268707e97c461abcef3511a70413562ad0e59399f48c40664da9076830ca872280ab29e3632d23674f3525b9657d3db1f9f292adac" },
                { "uk", "3602b6c3676caefd8e78dc2c9e5deaa30a2370de9f1b95ea4893042408342f5b8eb88ee2b43ed5f39ec855b7e87fe84b8193689ef42c470d9476d7c6084042ef" },
                { "ur", "869022a525f51d988f7846fec049958fbc1a5e1260294a9b3df6a37dc4f2fed65a19d351844eb838fad6d461eab5b2b6156753ac80289dfa4cfed3fefc39faad" },
                { "uz", "f5ed67b82902e7094b312637cc6ce4538eb0abdc37047f277fc13679b73203a809967a21c73066ccb14c45a7643fe6d3ee31d0262db05f997bfbdda4432ff06c" },
                { "vi", "d8d3f26c0539f2985f0d3f3460d3c2c715c891fc59334e0dff11752c8554197f0b54063792b6977e9447061530ec944b0f68b3cb95132398f44b1650a8134d87" },
                { "xh", "d5b589754fbc5ccaf10d887b602d697d3b45e91b004de77301bb292e7e8e5b4df6cccea59112381d2698d47d13618facc48233eb1728c2f3469cf37c413afd61" },
                { "zh-CN", "8e07ee188311139d40b186d0cf76924436e3d74a244018c939a87715eb515a7e43c8467845ef5280715fe9944a965d8372120f7f02728cd879ccf8e0bc4fa091" },
                { "zh-TW", "de4f9217674cb4fbd87f7cf619be517033912d1ad549c388e5de5ec6429ec7824ae83995c316a89848591ec1e454c3964c186dec8e6a75ae1e8e6b47fde38739" }
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
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
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
        public string determineNewestVersion()
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
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
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
                    // look for lines with language code and version for 32 bit
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
                    // look for line with the correct language code and version for 64 bit
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
        /// Determines whether or not the method searchForNewer() is implemented.
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
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32 bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64 bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
