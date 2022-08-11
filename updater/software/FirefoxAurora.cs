﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022  Dirk Stolle

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
using System.Net.Http;
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
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "104.0b8";

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
            if (!validCodes.Contains<string>(languageCode))
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
            // https://ftp.mozilla.org/pub/devedition/releases/104.0b8/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "53888fe130f172d66f4de2e619e5311b4f449bbaf58efde230242844c979cb6a54099ec6b87afc4fd65ec1207250b6b44759a153263623bd0f8b598a237d2a91" },
                { "af", "b13ceef80f2b4791c29075a5472e6fedecdbc28c12ee5f388153cc3efed4945c3e450631687e03ceac4b6fec5dee35480fe7321d62f4917694be5898ac61898f" },
                { "an", "a069fb40c1cb2279915570e59c449fc801450ffce831d3088371827d99d12e176144bcbf53dbcea8b328fef44a8ae4e7b7fa2ef28db13feb7596fbf3fbb81e35" },
                { "ar", "e91235801a9ef126b2e333af1f51dea5a57d8983c4c4ec81ccfa862edd48aa18449fa80e1edcfe958839de05959ff63f1ad412a8cdf2b2d1a273c3246277b1e7" },
                { "ast", "85fd2fe9ac654d328d30c319554bfd69be5369a08ae4ba71c14a78b38f839ef24010b51876f629d1e7e8bf3287d0688c4e1bdd26522fe2ee25fd8dedde17f622" },
                { "az", "5dfb57db939856df4a03f5988060336cc248edb3d5f06b785b46a5e35bfbef52446a619d0cd1fd6689a335fe9555528bb6fa6f4449472040193b19dab6a27b8a" },
                { "be", "ec60b2abc22fcdcd1adf3a846e65f34eef53199f52886656122c26ed4dae61dc42ced84c6580668e0fa5110f20d8909e5dd2d010548fca42c0358aefb2494659" },
                { "bg", "876f9990b72478b960424233cce52429c3ae7c143395e7f5882acc2bb89a5e56486445542c73653441a539d1268218f241e8da30e56fc262ec13cbbd46c369c3" },
                { "bn", "d205a6282310f0492d9b7ebcfc26dc71537c4421bde214b0f180380c4b7ef7edaa44096acb040a2396ada2ba003dfca5ca0f0e3a12b46494c4f52be2dc09f43c" },
                { "br", "0a181c38dbc3b1c39458fcb7693c19bbfccfb478dd571c738c2b5129df0fdd4beb122dee6b7f0c19749dc6cb7a51a35e205d07d3797b24435acef8331aaa480d" },
                { "bs", "46ed8e54281e18672b60b0625c73ce83d78cb2d5369b6af652e1585ded596d821d74beba76bcf89f5d9eeb8f7fa8053a067bba6fc34b609598cc71cfed854858" },
                { "ca", "538bc39ce825dc946a6292818e855a4a92d4e36b650cedbee715e42b37688aa45824e33c296494d3b2bfd0d997f470d9361957a1bbe0b4e199e27adef197961f" },
                { "cak", "b2c85dce91410a2c9806d64a6caf2622c97daf7e8cd01004f615b415ebe82a9a0dd2c87fe9330e3d1fe328c8ddb23b0ee118f2a41a7f7273661a0e26a2bad98c" },
                { "cs", "e8df86bd764f5aca082ae8ef30cb4af378ded71883fc48af33375f36be35f1f5479af12b9a126c7a972ab93bf03984b859d9c72d00cc89312bfa34e0f1b1fc14" },
                { "cy", "64f2d9aac38531f62afc8487f2be2c96f3a42dd1fead15dcc58566e73de3889e5f3a2c4b29a4d86f0a2ac7e1ae41279b07904000b7e14953f4933de5f8f5fa14" },
                { "da", "70b553b1ae72f73f49b62ead85b3f92d370ec50eaa657a55ffbbcbc9272b55a7cc2d2c5e702fd8f6a5bd5e131f3888058f5cfe69f3711167de423821adfd06c3" },
                { "de", "f4da97f91828b3021042e9ffe4ef1ea312ec751698877827ab2b2849b6ee29820c9d3990f8616f91cb6da20b26a6db08a1f9e667b09041c394eaf5119524fb39" },
                { "dsb", "fb6fdbaa2c3bf41c1788d6f8de6627467a7c764329349a2d08217caaf46ebeb4e6d5f75fe16ceff4401ec51af1c1820538ab2d45fc6546e3aa52ef3d6f621ecd" },
                { "el", "9c92221ab0ee138d0f637236fdbcaa3aed86e0c46c28b845cf5b76ff733d31e357bb039109e11cdd89589d6db966e20334fa2cf8e1b4bc78fd0b87cc85c371b9" },
                { "en-CA", "ecb5398f5fa452b94fda314ff61bd969f2726ff16ac6cf643404a859c39ec883ed368a4f73f33a64445af4248aa45210f5442dc230f7df4538ddc7288e5b7761" },
                { "en-GB", "8349c426098f0461c9921118e05dc985531304e34dace602ea9273c511c185068d8f61283af3deee633d1d123ce08b39a07c9a8d179cb0aa69bc94edeb48ca3f" },
                { "en-US", "6cd1c14c59f5de2f92889c37025d08585cdca0cb2dd05c9141178244c877990c33f72f0df32d9a0481a8bdcc9714f55496fdc8f25cd496f250a596285713ec87" },
                { "eo", "55a67eb8d1b137a527488b05ee810cc5429b2b31efb2b15dd8d4b3ec23791cc5e15447b9f90a20640e2d94c376aee343332d13a4acd7b3fa0d29a2d433c5fd4e" },
                { "es-AR", "453dc126584219d9a6ddf21f912f903587a0cc4dbb707ea6401272a331408f73b77ef3082ab144063a3a5c8be6c5f4e9ba7b19e4cc2c36ddf50b1e5cc88232ae" },
                { "es-CL", "3b51f27fc1dcc8cf52a9e0c73cb389c99414f9aa7a26e3dbb67f86f0c5bf112518fd152df8146677d3fad5d7cf43f2422d2e35ba486273038debff4b0fd99d40" },
                { "es-ES", "c235156a2af16cb3118a1b771a1b14206fe8fadba7c8c47e16fdbf992b856fd35454c1a2f8ae6d662fda0588ff96045dfa12b19046d3c998cc6ac8411869952a" },
                { "es-MX", "c7c96d61afd3f7045c784af207e4a28c01fbd00bbe0915249058fea1b3cae95043113b3c96a6ef65aac4b7bbb96acbfdf13617def95605bec605e508e577624f" },
                { "et", "4ae6b69ae746c8d97cba243c8bbd0909cc1dbed489c3febfda9ffc5c93744f0086323ced3410757a47956d0ea41d898139c9c68cda45bb5f78c667d3c05f5fee" },
                { "eu", "30bf92a1081f653d01da4c1ed6e4ecd6a7da20b9a26f9387084184b8223650a5e356bb271c77b11e27891cd5eed5b362bf2d56ce3e3a7ed0dc5f481b136e58d2" },
                { "fa", "cad3425269c7ae50ea3c65a77472b363b5f5fd0e74c13c4ce80ec99a63fa65850fbb27a4cf3fe3303cfe068bf2eeb3a16f66f39dd2b1aab178e1d3a6d4ee153d" },
                { "ff", "d44819e6399447bdb0a27faceefbd4dc5d835df8e794686d2dd4bfc792fb31556f1e639a64cc0dd63b82d965fd0841e5a2dc83d4d3e3091398a4a94c9211a3a5" },
                { "fi", "ad240536eab44648381f793a7b019aa430b4b35aa58f859878233a20a28e3eae921f999054dc7492373ec97454a61469e0d72175f632b595216d7b14d0345591" },
                { "fr", "1f5faac2abde887770b8706fd3c1878edbc8bae66b208c96d0fb4329ac7511369d763bc40f9d349f492dbec5dad0d1559710082e3d1fe0b7e1e721b2183f24bb" },
                { "fy-NL", "f6e90a6dc7271599f0290775ff71a79e9ddb634e19cde8f8e0d72edbb5aec143b067d6acd5c322f767a489a682ec93d8012fdb71f04fdcdeae916e4c84291e7d" },
                { "ga-IE", "94a5a132b16de97739ee90bf6df9cfb402d8808a364143c066ee87bf9b44ef9dededf13a8fa2e6d602265fb4e4fd4ec3c6c479903e06e5e4a486cf13ebb29d5d" },
                { "gd", "0d5ff09390c89fc1d596a271a892e1da2603ef7549a130953bc54e681544ff39f8c7030f85a2e2223d26bdc5938634c925e3381a31cbe56c45722f020d4de870" },
                { "gl", "ef94c1c0c7e264075e530254ceb84797d613ac795d1f0a95caba28b7304d0a0eaba416e9826715021827bcfd4dd1423025a8c75bc783d087256ace41938af41f" },
                { "gn", "2eef2635291a308bdfbe8dbfca9ec5bc8e24f08ad0abddf812589f6b4cc736beb989e432cf7899618dd8392d401cb06b9c1082a09e11180fea4a7e4c23d7c788" },
                { "gu-IN", "d6de19f4dd46ebbb1f75db58f0a3a3875930b321b9b3a85aa7dc53b64963573cc87d56c9fa264a2054b37e940cdb11f1bac47889fb829fd32ecbfbe1c87e5371" },
                { "he", "4c85317acc831f41ce3a9890117000fddcb58e587958f5d2a766aecf1f547a6144a075733cb6fe3683a59efb683ee43cddfd74e9df210d3a069dee4642b75c7e" },
                { "hi-IN", "29e1bffdf9cabbe1e9c0bdebd7fa905973b2ed851485e29e19b433d0018d423724e58c470f00d0cbff7c408e1c2d2e766c72dd0201e8e14e16b88d03e7af3c6f" },
                { "hr", "6fbb5fb24fbf047b09f7f2e96c3034ab180f3c1eeec0b164f2c0329cf6e758cd2b5878ab33885029d86c9cf8f3e57bdf895cc1ee0a95637755bfe0a84fa973b5" },
                { "hsb", "8306c6c8bd16bf5c4e62a7ae7f5a015e7d7779ff8e0b33590565904164dc21d0c99965ca43d6f2ab768623db48f14fce3429b13147c7c8bb97c06aa9de0ae345" },
                { "hu", "bd1f567b57ed588ace2111cd7696114a55115481f88435baa75c9e02ddff4062659b4f439b8b8d716c9c132d1bf725c50052d77e86891aa863fbb389b34e5729" },
                { "hy-AM", "82626fd1124dd6bafab82f0a0efd007dc797e8f850982c0f34c486195669e0e77dc10056464702a05289b126c19026b5154ddc4426a8dcd3b2d694254eb01a9e" },
                { "ia", "a17570f3943bebc4d9979d221812779f7350775f1f82e8706f0846c3a0578b58c66fcf2abe4c80b282f43a5f7edeab6e1c2ede72e9b0e5a01d923780901cbaaf" },
                { "id", "be26f505424946024378f194dff7812ef470c6ce853b2f849fb000de2e4f22f04d204c40ea208aa9e3804ab33448f077cfdf9dca24c8a5e64e4aa9deaa561406" },
                { "is", "1c8e5ed7663fce1930acfabffbe5315ab542266b621c7eb86c1f443e43bde96a2c76fb588626fc3d12cb1e06e17fa6a08342599e9da11de365db81e53374fd51" },
                { "it", "be875cb8ce86dd37c163562b939af9aa2013fe96c47b658e4e3bc9166876bf6cdbe22f21ed41d464c7e2fe66acdb65e78b0e010ca18064bb463b3e5a1eeb26b4" },
                { "ja", "9fe5d5622a624c2c488fd00a2c62aaf169f4c9258a71aea33bff6bcbe0e8eaf2cc5d92343fe8d05e97bdaa5f4cd004579bab6aea18ee858702777e834dd199ac" },
                { "ka", "2e6e846d7cd2ae70b95d49d5797ebc1a16e4cfd8f77622f61bdf3922a2a02cead45dae7fcabc31f2a24179b4a10cc4bf28c76bc4c91fb3581d203546ad1fbec5" },
                { "kab", "10c5f0d3628871f59163d768b92db3b9b3a262726cb84a1c5efbd86327a4c2537666ec99a966775db5ecea744fcae5fa519c75a26912689ebe858bcf4309b150" },
                { "kk", "0ebc903a58b19032d340acad3b2ff29a47bf03bf26318fa966374d7b41e1be13830997a90a70c2dd2128f3efa792513e5ac688811fd22a67ae6abc12f14e1501" },
                { "km", "cc13611c9b7549e206491dd0332868d360dc45b3e6520c7e1b88ac0f498f9f23879e0393455a52b68b8ee01210e26afa8924a427fe2da414eae75003f4923a06" },
                { "kn", "91bf549b771b443581e276bad6bb14f3c9dc2e8c8be3887ff29e5d7440673f42200ec3da0b8e115daf2c16fe32c90fcbc7c0f6bc1e19799dfb589b206c3590d0" },
                { "ko", "26bee8c9f4cb30d55c89a192591c2e008eccec6bf4b3437d675b645388cd65a9dfd7e5e7c67a444a518deef56946796b84d20dd9c06575861202ed035c23dfdf" },
                { "lij", "5ccfd67a821152c6f40224620f2f3abe2f3fd713127b3614bcaaf77f2780eeb0406abd56a76e677b25860535613ef9ae9d1b650a7d18a1b60ea391413c7ec062" },
                { "lt", "61388fe41b111b055868e81000c6afea6aeb63065977f776867c0a505845557fe8cf76b4cb15326d2f5433e74a66de1ac452903f2517d4c304ab25e046a6657b" },
                { "lv", "55d19afac189a33aaa84e0f0636de70fda31253bd0e38a1ba5a9242e8120aef9f6d7acc2cbb209375f736292353b3132ba1cd471e494892b5f42e8c54c0952e9" },
                { "mk", "a5dde93bb994d14008095052c1dfa199a561fab01b7453211294e621588368c2da3c4d0fa12d512dc2bde7034fb978ea950978747cf12f6183a07054cb442d70" },
                { "mr", "776e122a0096196976eac478507e29b8a5694942b0c1db1b85a74a7bb45d1548a54884e3109d92e1048178bef3a2cc507f52830a12026cb32e842a6549b14ee5" },
                { "ms", "218383fbf8369612fadf3b7cf393d53f1193befab1b14087d06c05e69b0429e7e2287df208770bab17dbd778d3d293cad2a28c638dd194c79b618c625040ef3c" },
                { "my", "3ea1013c86d6bc3f8fd528ce052180a019d8c6ebb02c05a9166094bca04f53820244314cf5bb766da677a4fc0e7a446deb78982565e1bf8012dcc1f70f69b25a" },
                { "nb-NO", "52a44dc287809188d24587f58be2547fe923965d2d3b7cd83793726ba168f7c15a450f75d8622b95ee9e684f25ed3aee7f35550422a1c581f69e0d1334ddcf74" },
                { "ne-NP", "8a1f36b82dcad8874fc7590cd6bd9588186781e1a86b25e469140a01bbf79532b728d16ab481c217b676bdba7956c4e1f0354de70abf06c325190838f6a966a0" },
                { "nl", "d19dab3f9880a2fd8a5e6910c0c7fe0379b0b2bc1848fa51016ef6aa47b703ba4221d92773f0f0858c7def1274f6ad0ed801d7e43b3e45aae782bd83565bd6b2" },
                { "nn-NO", "82c76db0dc0b52229683baf130dd7faf92e58beb2dd3935e1ea75d73e5ec8971a07cbc87241a8df14ad9105d7d82dfc58065f8055b9571ca7b1379806375a997" },
                { "oc", "df6a61a66cad2ac970671d8babfce6d3d0a1a256b10c4ae035993d5a0c461ee087332cf454ebba185103111ef0a33a780f81bbbe7fb5013cfe4f34d835421fe6" },
                { "pa-IN", "ad5c81264e08147bb65cc839b0f3ee229e786c891c37734fdc05db891fdef336fbfe1d71b92cfdb4ea935d5f784480bb1deb2a668c010e2b4d8577902ae17109" },
                { "pl", "20a50eaf27e299a7e8f0f81a40db6982d280c376e1209b526d155ca72d0b184af969c8135a269c9e71e2e070ba005b6db9197d281b03866dcdb0254d5b78e191" },
                { "pt-BR", "898ab25381b6e02606030f5c1398eedd0b4ca36b168ab3a47b1ec1d21be1752f05b77dc123ff9d9bf71da4edcba7f290f6dcb295a37d2e44f68fe907040e82a3" },
                { "pt-PT", "45efe520124c701c4286adb01d5bfca46065c487a1d31b9f42c6a530059b15f2639f97c232bb4d8d0fc50f4e1b81082b3ff6770dbd9680deaf5465d75e226916" },
                { "rm", "30bc9d366ba651a75ceb634a389d4e5c00efc5d375570c022200b6aa6f29291dfcde3a4c3f619f822ce820e2fba4c60651e5924460d2f51d3032119c5d29b9c3" },
                { "ro", "9b1b39b13e34cf8f2b1c4ca0220adbbcbd7a18535be78bea9666b4c8b8c580e48cef7a4b227fc4088ab7a7a40f64e34dd688c1bf0927fefa8c0fbe3e2d4223d7" },
                { "ru", "1124489cf29adfebd802c9cb95f7dbb4b26d327b607c256cabc8689bce7b54abe591755299f7ea092fe2a39999532c9bf40e07cc438d5f3f9a09fce369ae70c3" },
                { "sco", "2d3c707e0229489d7c4f653b86826f043eab1036bbb51bf55296d1c48dc786b8660c0e5e813ea630bf52c24ad40dc75c9edf120e92b6bbc1ce23d93e117b88f2" },
                { "si", "b433ea9e69b4493928820fa8bf35d942ea1f6e892414090d0f2cae28c8f5a7c1daf27082815dfb8ace2be3609c3c57628bbf46992742e95048fbd0132ef404f0" },
                { "sk", "a734b5ba935251a59d8a6a92e3c2407e76e42dfc8e5380935ee0b391961e8ead29f8bf10f8ac523c9c7b5152aacc2c007f399f3498fe0f786cee295f2c0d2ff2" },
                { "sl", "6b4e7bc801291b7b536d1c041bcaa96880f74640c1afcfca19c233f90848e41a1505484fe65b4222a33433aa6a18bcb5051b8064ad7ea71073f765eae11e6ee7" },
                { "son", "c1eac12fd6eed9d8854d6460287b35c5bb9fa5d890b33d7a4c96a4258a56d7aa3bf394a9a81146646c4bfa589dbdcd6fdee0bb9da1d5d6ddc490a8779e97f9dc" },
                { "sq", "5db76c8b0af11dac5f2c53da123a636427c9c7ff6fd370ed9882ba80fd522cc3e579c754f5db70273ad2f88b844419107e111aea9db18b865c49c09672083baa" },
                { "sr", "75fc628b154765f407f8b3ddfdeb9a842051e8ee052bb9b0373ce55a17b564789cbfdff1ca7581afb36f9bcfeff97b9f64fcb6f97b3aa7fc83d71f91d15a1f28" },
                { "sv-SE", "d33844c9bc8119c913d3239eff315843bb78d21f26029c64e2342b59d42978778a2c720dee4e52c46eeaca6ee7288186013590ded869508bf4a48541df0d1be5" },
                { "szl", "48d3eae5605234d198e924add796e8edb97b7673037d4fc8c6af55866e921eb50d16b90908ca39d863d1d4a57a07d1fd48c3c33c0777916dac5cac921a2b79a2" },
                { "ta", "c7c8eef0f165ac798e557bd3ff345ccc06ab644d350b2b34f905f7e4d46d2c965eefd729ffb2b2e9d7e8b3d1d451c1c5964c79d96b41b4752f122e0b14b4b521" },
                { "te", "e7a4fac0ce070757395cee7901113ed0bed975865478ed6d3c09a00a9175b75c8005812e5b1b98cd4e49e046219b178e331f8d15219610854ae151ae7e616a86" },
                { "th", "b7eefca37050f13350989e9cb53916ea077593fe0537e25adc4a7c5bb2335a0e073af57b4770c09e7575fdd64061d26ac9ff20cd04c106120720f187692a3a9d" },
                { "tl", "a1a5c0e8ed414beaaa326b17d72cc779fb90ab8c97fd790c7d82399edb2cd7a2934a6ae8203b9e8ae2b48e8591b5cc6322a04e0f51ced9d033b186956d75938d" },
                { "tr", "c0f7b7e7a4d0fda41765a940b471774dae4d9449a92934d5c656df26e8da2d33735054204a04a91ecd57fc13fa21aa028a0f7dc95b8a6e05de988567a965a67c" },
                { "trs", "18e2ad2a56d207676fec5df3b93b5a72e81dee25444f015f7a8aaa9a8abe7c75a2d67280421e0d1856778c65e27ba1bc43e57acf43311572f841e2e4b2470e27" },
                { "uk", "ce3541dd1f274029a2c22a998c7b9ee0da211d1c29c71cfbcee5d68601cb6d61125c7a31ed9d5d1f4c19ec573e336018612f391ceb20f82e5bb1c15a1c83db50" },
                { "ur", "c20119129376f1f25874ce6335c84923bf790c2a3f5decb1cfb5d9714337cae13746cc78ca878bcad266b8a088416ace963294f6087fbff7fbee17f344ecd1d4" },
                { "uz", "c86e6fbee5aaa1cdd212683def46ba4b1ee4063bbe5e444cb5222458a3eedba0936c497344102fe619be04fa367f1b6a8f33c2e1601a8ff6f5163e7aec1df75f" },
                { "vi", "d53437743f8cf31262a5b77f04febd8b33fa99018b7a03345deb98e5546d7d95731e07a8a17d02a0d527fd3e845ecaa03c20c5ea6beed8820e001f1c894abae7" },
                { "xh", "31b7b00788071297fd7d264e91fa56eceb9f9a6c30827086e7de2d82d31ae9adc22eaef2e61d774d9afabffe4aa75114e1ec91d5cf891b543d0b12f360e51d99" },
                { "zh-CN", "c68f6366253fb58e6baece3e3ef109f8b722777e65e14c4cb7e8e7db3e588e4b3e24cef389a4ce50621831a6a5035dace98b6f180101defd30ea9c5d4d08c06b" },
                { "zh-TW", "2c5ae5f9e0c9c96f0ebcba140dcfe52facb28f7c3e798a5d43c27877d0d3ad4110183f75896d70e45e3a07de283d52fd1c917a013cb007caf924071d4ed047a7" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/104.0b8/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "da993079d43ffa1be89d121a4e446be95ee5e84bd66730d148de681387402080079422e2d6bce011e571c43ceb951a8ef3305946f465da6a536b35b7a65a7835" },
                { "af", "f4065dbf7237d806ff0da9dacc92d484f2e6d17729c7fc4b9ab1bb6fa64429d66dc3e17c26edf95080fb305044844eeed8f326d886a76af20fd4aac276541091" },
                { "an", "7206d643e7df31201640d5474fe8eb56ef9cc4de7b0664e387190d94e2b6e28d3c965674d1eb96a6058645a23188d4e0a62850eef51d792e1db47549c81579c5" },
                { "ar", "b12c5bcb41012610e8c94e39fb10361620f94f36a3845d4165cc0e172a46a938b30d1168166090a53389b638e253926007f748f3e259738580630995491861df" },
                { "ast", "5d8912263c5ca8cc27d07faf6918850bcfd338437568a6183f5da0cfcf85dad9f2b761adb54e2048c2d8176cc993f3356bb0e3ad01f0cf2d44ba0a9b204f078f" },
                { "az", "30e443d2e019c2b56c9dcb20613eb7178787e4858708fce5fae4d0ac2624bf3d889a96056d26ba50d03ef2d9efd05709e0da98bd86e8f55e1319de9bc4a63f36" },
                { "be", "8230d584fc35219cb01bee6667010116e18eaf2e185415c9580b35ada808e92486d92c853eb7bbe23d5b20d0be8701d1b116d3734c4a70335376d76ffe5633ae" },
                { "bg", "813d21d01f64cc11d7ed0e969d11c7d9c8b98216dc7193bca438d9bc0dc07057ec63cce77ca92059959577c1af96f16585f2d18c4cf2d2c18380cbe8361b5cd5" },
                { "bn", "af4e6f16a2bdf068fb728af8f01c666dd6487db2f8a17b444fbd7ffc7b1fbef9d2b30e8b144688da1ef5d7444017b49d8ba72860f48be1f63f2237f1f4c991dc" },
                { "br", "5e59c85cfb03f02c4366ef7717b64f9802aa1d3f708e7e2dc808844492810e7de08ede4d58df5fe48428139a37bb1ba6974a22f4fcccd54c388087fb523b0ec3" },
                { "bs", "2f617f565f6fe31e8093754a562672dc1445e5bac087f2514ef7d689e1bb8b060ef94825a929343fc6e988139acb475e0feaa8654dbc9443db442aab8968e58a" },
                { "ca", "b3ce890a06af97849c8a3a0ffef5adaf50261405122eb8bea8cd4ac778316f5b99268e9320b6d53a0c6891eb783584a531bc4c308eaec60ac8b798b6f35554e4" },
                { "cak", "0021cc937dff88fed40c63cb406b36e55963437e08bacc979a0952334dba0cd67aaa23da14933fe95a5f0147bbd3bcaf3c67b1e4cfb1faf26809a3e1067545fc" },
                { "cs", "ff9d2436d420e7345f63195d30d03841d56e2651fd0bb420c36f681c0e1bba8edbbe1884ee849038159de7b458ccbce0312b7c17374a47eaa6e7a030a235acf0" },
                { "cy", "ff230789530e6755bf3cce64d2b4db42d1a1a0e02ae3d9f21059f0de7b8258e0d44b2cf8a44d2b013fba0691a7a5480481eebafcd3c200e4381c5cb880867dfb" },
                { "da", "814c7863b039ce8a9f0a1d87909880ae2acb601fd134daef2c1b8961d302a61e6bcbad686f34d68a418769903ff5667454466887994cd4ad0bb15a97ac124c1b" },
                { "de", "04a3bfd4630b8e7746410ee4ef30b59b97dd64afb21f0df043f107fbbdc0f66ad8c7e20f4c9776a554b952e7c1e1f2890aa91fed527d5633c3465e8e0b7552cf" },
                { "dsb", "739a60da0091607ccae4bcdf07f26f82ffe8af39a9e0dfa8b513becd9fdc6175a2bcb7443c9e4f2440742b0e19386a344c662f67d41e0b3595caebbc30d34029" },
                { "el", "4f550ec5e72521f8a5f2b229788133cd15f972d097df335ba8dc33179fc52922351aca0c9e2a275087a2a635b3794cc6448c53eaac0bc028f7015886e7e51cc7" },
                { "en-CA", "93e8e47ae340e7df77e47e92becdb31b84adce21300ad27fa8cf9b58f7e382ae2a31d87e8e964dfad333091361cff29a16c10f0fd96bf94ae1be2db3d8965114" },
                { "en-GB", "ccf9e7dd9b0ceb869c4d52e887bff5df57ef439d13474d8dd309fc4659fd4a90c028f76387698afe46c6229916a979fd68cee3eac2baece54a233e822074013b" },
                { "en-US", "91807de887fe639cc0a085439750199f5cef127b7ca22f4ef503e287d52f8b852df0f02ff82d9c8d2b3cf95ae58b1f4ed6221d730ccc4f933ba7fa6c7d5bc8df" },
                { "eo", "e4964158a91a2b63dca19111820c19e6f5ce8ba0140e54d66a2363b5d5d25eb099883548753b3e5decb56dc37667ade805c4df799a7599dacae1cf0b7e38db8d" },
                { "es-AR", "ba97cb19457928b817486a685a7cd7716a3a1e7c2050799f66981adbb281a501a229802966007ccd0a39578c43ff82a753cd9d6de640a3fa14ceec9cce85b453" },
                { "es-CL", "bf648eab22535f9197fe24a302eaeb05fdf69910907487d53b676fe50d7233e17b31477afc08dfc3bc94934c50fcba8d65ac34e86c75679a5405bb50e20858c3" },
                { "es-ES", "1cb03d1e05c672fdbd84b643d24949b4e446a0606903d23faa35819ad78c27d6929c67627f5d1aaab49d7f0643387892e8d348d1e5004d2152358e04529e8009" },
                { "es-MX", "5a10380c612391be4a68e5033f346e3f8516244e740d0b41149767423ff25088e8ddcc75fca9ad7deb8fc24d2ab15eaad9262aacb87e2136c29bfcac67af5acb" },
                { "et", "040132f8242070322f856e92a8ed6b1ecc9a9968abeb21ab2cafb0108b1f5933b220a8677eb7dd1372dccc027cd3d82f7b2a703ec13043c184e8433f1f8e41f4" },
                { "eu", "d63bc7ed8e51f750b4b269fd37fb053bec43db7edb2902c3eae3039483bb48da9bd65800786fb048c34f6483de0368f27150bc12ff3aea9fbaa9a877abc5632f" },
                { "fa", "2e87428cc8f2043dc9d47f7f051b0c5b413271cfe77c4f05f1553f4e610e8445c59f18c831481faf11280899131d872fa573de5383655ff07122114e0bb1dfdd" },
                { "ff", "605990c108fdd0d72bfbb4c78a33080e480bab7f31d8d9c6366f0a654714eb29409d89acb52a71183733e48c83d0d3c21f9588cb37d5671618b959bb1aa43653" },
                { "fi", "122b7ec22e452a6065d200de8f8bab459cc41696793cdcb4bfd63b48f0751daa15e4576ab922a154fcca7170107f9d0ec7d12f8997576169389d28d78588af58" },
                { "fr", "2a2ee53c7861fb7c5805b8d22be30b2053c512312d15c70fd57ee1b2b4478bc89f87404f4701cbdf347bec2ffb070132d465fc6f1f3e922c20897df65fbac7f3" },
                { "fy-NL", "c10a62abb19fb596db86f8713b5a9a3fc558553bec1ee19dcb8ff75bd2fa61c7b5bbe67b1bb979ef945f15caa941374e4ba6e87ead7f57a1d51f0e53ba0375f1" },
                { "ga-IE", "5d15765eec674d6c9d75b25a2c16b0438fd142f0b6631c63bcc4d7fbf38ca8fbd290277a00a2101679349ffd73b73a318abd417de940af3d324d187a972f748f" },
                { "gd", "8b0d7e5a80d04bf4d007b0c5214295f766dc786cf80707f652f3b8b3e6f9961a45b370003acbc61fe77090a9f1d96e2f8adc9157e25889bddccd68305a84c9fe" },
                { "gl", "71219eb2d144ff2d3cbcb387a792243bb37b1e22c956d31c86027e7112d83d5722d451dedeb164e259b71a3f84d6982f64246ee929a381815170d32caed30575" },
                { "gn", "3f5d6af7bb132268e0040aa0c629e20f5ee033f5ec405863e7206ac8632e1aee433d1f817ee1bc78663720ca920e2a63acef9d895f2d510979f32cc2275e7569" },
                { "gu-IN", "5d3ebb5375ec0c7df95c2ec1f4dc93782895e11b2ba55aca00b0745a5f2dd1670ebb8aaf90a24a359df56c7f5e7daee42c5b9a9dcbace9077583ad29ee057aa2" },
                { "he", "50f2b326eea61ccd9f44a599cc52b8e9adb630be03eb8eed45f2c7bff23a902c497b3dbd2fcb4869b7e5533245e0b140eec3bec85ce05724ac6e38e132fdf524" },
                { "hi-IN", "c58df3adf9e8f3c7fffd3f8dbc3c377a96ce38acd85c8a2bd0c04a9bde26cfe127a3b1756e35190ed21193348357d9306b47afa3d6c2bed86ef58437ef19124a" },
                { "hr", "6c0f88d39c49a596499d6f86e2a7cc17b3ce155aa865b670deae8abf2dd4534762eb9fab4242b17031ec3b78c6c00b3c035d95d2e01e66e363a0cfb152bac75b" },
                { "hsb", "3cb6b84b58a02382186a061babf4bf86b81d02994eea81e4319415ae0bcb0b1a4925564db2148fabc1eca8c036763e88c4b24927683da28ef8479df2b107e9cc" },
                { "hu", "1b98da54ca9bb741495dd72c6b71f46440d82c39c7fc2a23dfb3af012ec86cc447da7ecedcbfd60f380c66142c63defa9b4a5f144bef8f1a41f05d56d09b1d60" },
                { "hy-AM", "ace0482dd4fc4a13159947b77ed2bd30706469e8f221169483316750520a9b96578576e4e5eab605d8871a64f97162234c2e2b8add737645f7ec4325b260b1de" },
                { "ia", "a961bf623dec7e068335db188e16cce57ad5e4ecfa7ac455ee22d34cf722faf1799ed3277e89e84412d28a051c69b19967ea0805539a306b064f6c4b9ae92835" },
                { "id", "373d1f94717f3846e8f6346e2c7fbabf6937099f42922577ceda1c9b9f5b6b70a469a7b1d6b1c8a9e01b3d57423bd9a68d588b30d2cd166daa1b54abf81d67c3" },
                { "is", "0f7e9b6e9a8ea1e4632a0187736df303b7d06cd961d112b3c7da7babdd0399bd52c9d2646199802e7c75c334f721170217d6880661fd6cc54a0ae158f56101d9" },
                { "it", "776a3fae8f83dd1109763321cdebe025f5f76ff7e9f784799bbcb241af868b12fcb41dff8266d8bd799f56b66a08cd0d16f89a22a0cb7297d935f691e2eeeb38" },
                { "ja", "b28924857a23e6de2e1626f35acf2d6ebff1b0a99979bfc7df8ae1bf59a68ce957ac78415145781bbd22e7fdbaef773ea267ec1912b1e8c70690364da5b84404" },
                { "ka", "e3fccfe38710425eca5af322e71306e9799f96524a342a713181e3610d427ba73f553f12d6e352106946d5f818e629c566de2e78d85a3a4af90289fcab3ecee7" },
                { "kab", "4c935ba13b8a2d2cc0b49829079e4abcc1b697c87ff63a019f208e4060ecca7759302b7bda8de627dfc559876380a28bc2ddc5c002c40210c2c3b38738cc0954" },
                { "kk", "b997a8b8dfaaf189f44020549d743af89d5239d969eaa490e32731c1d647ea8de6e21d6f35083ba5f788f07b41a1aab1b5fd593c387a325557fa4e02e829b299" },
                { "km", "545ad2c3f5a34d8cdfc8f001ded0f8d3b9763e382b4ae51b7c7fec659c6f7974ffccc398bac6382fca084941ffaa7e5c48729f3d4a4e177d4e30c67dbd121422" },
                { "kn", "deb4af29fcc77a6f4555c11ca34991a9d3915c49dd7ec682c85eb6268989ea16001ca185af9fed0aab5d9acf39c35970a649767e48ba3153be887b35c68efddb" },
                { "ko", "d888871ff7225a7a9672e016cf3e1ea75c797355f2cdee7b7aa2a17383045f61445f27695b21594e524b2cf7dce842fec1cb342d89c83a105f16e155aed82a27" },
                { "lij", "7414743def36208a255444d3204ebfe714bb439ad374214a75d404ae3888eb6f93b8b8b9681c7fcbf20aba5fd24460b969fad235730fb1ef611d36fdca44d50a" },
                { "lt", "13dcfbf129860bcfb94405e7849dd0c56185b5b0bf2b4eaa308d0066fe9d499c07d15ac342a2abf3f7a8e0e2cf90bf1fbe8795952c357a876e9dd02555e638cc" },
                { "lv", "7dc11260dc3eeac0886b87294396faed7e224898ebf4d4ce208f64ce766cd172e2a6eb4a9b2a8b9e5817919341f20ec2875bdf423295ccbc62c81cdaa3f4f9f4" },
                { "mk", "c35d2ed54c497dc240ae620cdc07c14f4c17023effacb8eead543493083a4027e7c005a91108b3b2690c65530c0fe76c78bb48327b76daa64a8b4a8c617fc2bf" },
                { "mr", "172c4bbc87615c9b78095d36a8ab31a606f2c137327ab862f446db7091f3181ca5e30f9d72e14538c277fdfed515a262731bb2ffae6ed4fa767cc469602a10a7" },
                { "ms", "b4e96903dd603197607be4dfdb67e48bd667f9e17bb8b5dea422379c11bf63d2338f06f85540921d0c50e9716217c65782b3db2f98280e961af5a924ac858383" },
                { "my", "bf91721a2c5057c8c6099ee76eb02f2e70ab2e4caf357da1db42762746c3f4fc6f33c364bce0ad85d654c8cfc434b497db37a5af00e01872ffa68775c550f2d0" },
                { "nb-NO", "b34d9da84ebaded755b55e5c2d77680791c9586d8720c1e19e1d4161a87966fe80ee2b0d873db7a186302a711151ed7eae2771f0942f5105f3b6bea976d59bbc" },
                { "ne-NP", "42758438ef301ebd27dc7f7c6394baec4acb60f1acb05cc60de73eddceff1406a995306c38abbd3bdf383e7639e360896f199b57f76253a5b4637d48280c16d0" },
                { "nl", "96eefc991c585c37d3801b2f0ec855679f12aa916dc4179bb84ccd39170acbed8cb5aa67aae0cb391dd15a77021a3aa1783a8b4cf8c99339e13be7078e9d93a7" },
                { "nn-NO", "537f85ff1c3af51f3d892b5540a7637347967bea24eefa9b1579538a05be6ff072a71a9da06d2a90ff0a3c7b29f8f08b639ead09a0a26bbeb128b2b01f265785" },
                { "oc", "f02efa7fe72afb7e642ca363bd366efea75458b9bb428f22599beb8c8b3ed0079e8838e857d06d97004a71bc4545a368dae9c62d3e60d1737da2267d10ad6ca4" },
                { "pa-IN", "bde429d5a785392dffd0c0a6d41fdecaa6b228b2ffc3e1094aefd2d831a7444626dbaaf51244a08b6a5910b565ed9c35ba9eccef3230055bf0580651f188705c" },
                { "pl", "a3f781f44df2daa0416baf06f4a20016e71a60289baee6eb0bec930d28bd8550935ecb065d839aa5b5899d95d1af860bbc28b7e50b9fc9797257b606e4f68389" },
                { "pt-BR", "b8addd5ecbaba2d66d01516995cbfd7f036a881b57cee681e2fb625b735829653cfe1bd559ba111764a727bbaf4659a40764b1d45cd1033585a0afa5b8a1893e" },
                { "pt-PT", "7eed82bd88e727035ba57a672cd726c83a9b7d1e7cef40c6863355fd30f373386cbf1b4b2059ac610537f17d4e991060b2a439b8e74da5eed5fd28d58888e17a" },
                { "rm", "e23c562ffc93770bed2bf0255e813728b36601d3e30b188f9a391c695891d048c72747e0d3816e296eea63795abb394cb786e0466b38b02a17391dcf31ad7d91" },
                { "ro", "faf6a277ccbf930a0dcd286c27b59005bf7bd84f0e698e002ca2476e450fb2d99c6d377c66ce33c9973964618e09a300e504e468e0b6fb42d2dce2e4eaf5d611" },
                { "ru", "2a32d821948c37426120d741ac21141bcb69257afcd5739d4a9ae7a8a1d6dd9c39adf51ad5775580d2df631604f43d2ad5203575e6fa916731864f2140c97f23" },
                { "sco", "0a8ba85df77e419db22d6f94a7a467500ab2665de079532b43bbccc46f4557146fd5a9bc7a136eb0022fb8f359b1e8f0278ec24b7c5d11a5acb5c268fd969e45" },
                { "si", "19325a5f0884d4e5e9b11a6065c22143b58db27d11d387c1c99ca1abccae40ee5dc07218273220418a8a2d6a5e9d85fba755aaa9b45a5155488c054e6eebfa49" },
                { "sk", "0c6b572ac0367d44d2c23971e6c09f13ff2d9011ecb99e5f7d08fcddfcb5c31ae408057c3bfe4164dccad680634aeb59f0bddb7bd1544396d6fd9e2fdae34fe4" },
                { "sl", "a47ab03f0b816fa59dd360da6e66e2e43a048504834d982ee76804a152c607ba71c75b669b2f6a428f0930298b3313ede62ca0ceea44214a0fcaaecae6093116" },
                { "son", "f8cf207d8d74502ecabc503e3f708c056ff1ae9e90244c82b2a92f79517ab6ba8940d115359afb8087e1d00e098997ff9cc653ddd95879d1e6d0158c18579712" },
                { "sq", "ed8731cc7a326cfe3f47f3e06a272a648c077e74e613f8af7a236cfc15c9614cecdea9129be23d9407972b0ae4349374de197f3b0d9dd7b3ee2f53dd9bfa9a3a" },
                { "sr", "91a3dc66b8bbdc5a45372152b8ce135cf24084ceca23de1450bbf95e840865c56f3fb793201521bc3bbbb2e85c4f51026494f5a8e09f04d362e3b909d8b77df6" },
                { "sv-SE", "90e9b425e209100aef015fe2bf450d7c9fa15b71bf77067a87f7a26c26287070d7e24e3012d640969c99ab440496fcab524bba86773b9a61671c624268fb52b0" },
                { "szl", "4f2f890f62bf2a2da39b027544a0ddd39348223c0a33b726ad8af478ee03e53f09eadbb48d8fe6f40a413ec30e50721b9ec06702f592a77f4da6a74653ce40a8" },
                { "ta", "dfaca5fab30089f82a954cfdcc41769f6c066e1dba0b8b64032657b84808b15e4831f93b884a0f6d8def01ece0e5000649ddfc99843bf8eb2bf16ff8b33fbc8c" },
                { "te", "f3c60325b90a56c5c5167747714744b3f2922819ef486ac9fa282333b2d0c4ae130a96a6b6f4c53f6141bba2a13154674c8c9fdfd7cc5365b53477f6be262ce3" },
                { "th", "2eb0dc4ab81ec44786a3b4b7a60a9eca79a9a653db149320803289a13394dca5a3c25af7f321cb87e281e155786b0d80b26b2b844c735b6cb2f5dc1ffef150d9" },
                { "tl", "55a3a79e466da524aa486a3ada5d70c97ad91e48bea96b2159132d58b4f77b438ebea9de49d3f3e28603b76b16ca200aaf9161e627c787d1936f4de4a4e3f3e8" },
                { "tr", "81634c29b032739784414dd89f6443b623c8746f269234b580a3dcbf4b8679cfa73490c1097de6c7fec0006e25b4edbfca8643cc6e9412092fbbf00a9e30c192" },
                { "trs", "258d782c1ea9b5306825ccb7800d71a1736f0773e0e6dd5c89d5630c682919ae89cbc6c2688054c13b637a50a40c393fd115f855f2ac6ba6882635667c219118" },
                { "uk", "9f514714410d0b560081d446eb5213321d60a6326445bc28984645ee8a7e3f08feb6e4dc5d2ef4034ae3a8aa320349886420065b5e54a9a3b9c95077ceb19b61" },
                { "ur", "9fc1a44f66cc98dc88a39e9491bb97ac5c1924a932fe1cc68c5513eead77a95080e79268e36f6d5517f991830aa60a8de6cc37b63d23a84972a442d69e067bf2" },
                { "uz", "a870ce5a86d0f4a1b9234e2201f4c22d9a8a77514481bfbadda7f5f3208eb02cc09ad520ffa8b1b33c5957ca2ddc5f177bc6ed5a35b7794ea773678df61ddaa8" },
                { "vi", "d93360c80e4ac98e3340866777c4b52593133271df73eb10871507282a2708898e3124055497bfc5ad8a0a439ea70f23d3b639dd8ce94b09466e0e9a3f7ae956" },
                { "xh", "449859b26f23aed3408f38ac089102e12e802b866a820adcc85bda940641014857ee792cf63794825cb65e0002e4dc03dbaf2d68fbf622619f3b366287838120" },
                { "zh-CN", "631953e1e70b047a9cd45cd6b304b6d172f8ac136bbcd57cfc738c8e6ee3b1ecfb3196e1986cd37b4dac3a1ca3e913e8041011c78682ec669b090cfeaa928d81" },
                { "zh-TW", "29f3119a062c7313f7c0207cf2cc279a329e420249d93f4df8c9365f7afb297e0aecad9c418a04f2a0bf21c401df239b244fb614262c5c5b62c0001e8c01ab31" }
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

            string htmlContent = null;
            using (var client = new HttpClient())
            {
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
                client.Dispose();
            } // using

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
            string sha512SumsContent = null;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                using (var client = new HttpClient())
                {
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
                    client.Dispose();
                } // using
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
                sums.Add(matchChecksum.Value.Substring(0, 128));
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private void fillChecksumDictionaries()
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
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value.Substring(0, 128));
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
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value.Substring(0, 128));
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
