﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021  Dirk Stolle

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
using System.Net;
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
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "90.0b3";

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
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains<string>(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
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
            // https://ftp.mozilla.org/pub/devedition/releases/90.0b3/SHA512SUMS
            return new Dictionary<string, string>(96)
            {
                { "ach", "b2cac43a106f71f69658ac1fb7b15456246bcbc6af46ef15f42a5902c29bc0b3e9135d74dca46d0082f7b72ffe33a3232f9f01a8e0616b8948119c2bfb9f1d6a" },
                { "af", "796608a2b523fb366173d8f5935a6dbcf7829c14cc29126c0c9bd674c3f37480b91a9c7edb5fb270fb62ba1a605759c16a0638adf1ddfa20b656b6ff479a660e" },
                { "an", "5c844a3d8561b2ddd66a99c4b250c65ae5d0882411367e92157f4271618eef7b8f5507178b5b9d2db21f16a3ba51cd73b3f7f0f2333e50ed9291cdc7e3cb04a7" },
                { "ar", "30a070c3a714f370c08e093c4499710fbb2253ab6d446f22dfb9e175f85c63d7b5fde886364f4f07c8271751728d5b13c7755e6c59170a67728a2e61b2dadeca" },
                { "ast", "064f01e57a8669091264fdc648fb30593a2ac4745768cb75b21b4f11b9e64d6d5b122e37cc109b6c60ed74a09f7811fb9aad2e46eae84fbb920b0346d92870f2" },
                { "az", "62fcbd41d33e98a3e268ff7a68812ec59ff9c3ff5ec6e65a6edbd7e5acc279ad2165961b6a417052d1505b87d1ff80899b91c9d267cb63618b75d6dcdb554338" },
                { "be", "60b67cd3e0ecdee3eb358d5c6a44ab976882e46c9a49b7b75a91578cdcf3fe892380c2ea2b7c8eb08426dbc691a7179879aeb10227b854523b6320d545ed50d1" },
                { "bg", "8918d027f349576f9839245a68c594b41bcad008b7a6c3e1cb79d79a795aba0f6b5e57f0f4457a1e4107df3c132557ea372a6814182772ba19a1d5a00179d7e9" },
                { "bn", "cbebe68892d3020e66ac9143203944b168b5a6e02394811795181d4221a3b1d19664cf189b651dcaf334658798e238aa6f5031660e1f67ed3bbc4032eb920fe1" },
                { "br", "497ff94b71f3953a76cc69e7097b96e48cc96e4d7b607a6bd75b55886d8422a7ff9378abd46b511e53cf25a2d62957be8a8f3246e8a27508600748d74c3457ca" },
                { "bs", "b9654775f37ceb189c87129a79a33da71a63f81f372c1fed9ab9294d876e1b755c6b1d5db1013c31224b7f70d1f55fd9eb34f35f731b3f7fd2ef8678e1aa07dc" },
                { "ca", "1ccd643cadc78eca59d2bf18f05053f29dc2d71f7388ef986418a67f3d3551a7bbc63d46fa86d6b2618410628654e40ec54d9bffd37e2ccce0de5928bab8f8ad" },
                { "cak", "7fbb4503db9040c4ffe4e42891643ddfe6dae0233763cd1a96476fe4ed997d9f1b37ccf7e2843541131d3a448333e807c570500de73986e6b326e2729a621031" },
                { "cs", "3375ab386004d22bbdcb2f5a52148957de7819aad8e215af97624cf347e0405f3dd4ccaed7cd596d600bfe6d6b96fb8b3db041d5de5f0dbbf57478def683a56f" },
                { "cy", "9dcf2f18f6d17b5ed621655c6cebc4c7dc72a896adab7b37d9438a60a0507a10e048bb2a05cd6f842d9ffa9e73923efbc6bb3a2da84d00721180b720c18d4938" },
                { "da", "47bcd35021825dc7809955a82004bb013ae67f4298633ab93846d2998acf42de1b346aceff7500c87651fc2f1dd9ee33bf69c3cd8d90698e6f93bb8af007b4ad" },
                { "de", "8981120015dec86785308de85fb96ade75de12f92cefaec034b172b23cfecd42377a9154024a33e196bb8be48568dfbb52d26698583bc4199fd49bdd45c8b75a" },
                { "dsb", "270d4078a1f92a29aba41a6d3a05436bdd04d134fab16b2b2409d07fc02a6bb1319eada4129bff8fe9581f9189ed58dd0dd0c76a3ef0be6f4e06621413c1cab9" },
                { "el", "c779a3ffa36a9d2a12dd50e5850942c796c4b8e997c9fa0e56937ebcc2f4c979981336539c742e63777104a9c2d393212db71dc363a091307d7393058e20fbfa" },
                { "en-CA", "46db0bc0a83b81b8baf551319d0d7ff65f6f2645fd23a220fa9797cc1d4a0956454bd55e9e0e45aaaa018b16cfc66d91df1712531bf5637eaa69086015fbe0c8" },
                { "en-GB", "324bd730bc0e1a35a459693ce846211bbbb940f1f27f93ce99fc377440d3cd9a9a2e1d1f1b96932385b252614622c342d9d7a4bc39920756735c60b7500201cf" },
                { "en-US", "552a2f22529f06d11158a62aede539069234862cbfe65a37c24e471bce3d33c0fe36baa3f64c3a7307d0543cc0d8bffa72ec9681c39b73f6b0d5116f1e565660" },
                { "eo", "4addb86b695f494c2d2954b45e58415517a726547ffe45906c34893b0cff7e341496089b708cac64e66d64da5f33833d77a791ffd7d51b8ba1e5571a2229b4c6" },
                { "es-AR", "8d2382c95561b5603553c2bc55ebcb8fcf05072163a356687e69b4eb540de5870e561d862f5839d1d3e2e26ceeb3c5504098ec7284d7423b037ee8503bf35b32" },
                { "es-CL", "aea7d0421a41a0be1c354102af57b55708590f84bd22e6ae1d6863faaaabf019154f7c46fe3ea004a94ad29a4547a346312f76e27916ad6be0992faa713b9daa" },
                { "es-ES", "d63b5b3dbaf00295a769e0572d30f83e5ff0b9286fb3b33800e71f2feda5166819171d19e657407e99221d7649a106950027a1cd97b03516a4e5ae63ece13915" },
                { "es-MX", "90e2688da54ad056f78216ba3cba86a270374990e18a3f8467044b9079aac17b6d961a054115b569be9e91908a6c3a11216306eb253d4bf3951bd8d66d7d1fe9" },
                { "et", "7650943d00dd0725f9dd9ea1cefa7b6c6e00d233184fdcf5fd0991952255951bf9700cb5f3914efce0e3852182572996ee5049bb2c5919b4c6ce92676833a709" },
                { "eu", "76d0beef99da720aee06f3f67b638dc06a6c995be0d262812a99654e8150edc21110db93f8efbee2ea41a19ee23bb3f40a19a576bcaf5bb11e567fbadc10d4b9" },
                { "fa", "22c85b8c09994ff9bcb507ef37bb6349117c529169bfc6376f8c2986fbb7925d236c6503346d061113fc08106b57dba61254d191d7030d48f4576e404d20f496" },
                { "ff", "8b9d4c9c8817aa9a0e26f436d9039f3b092e63329a37027f66d79956792470be6b1d56327821d921465d956045748ed665212256ecc11766090e4a9dce34db36" },
                { "fi", "86d9615a5eea6683579d1a2b9995f7ac434587a30a068f6e047a7dea6cebcc07dbc22dbda8b41f38adc25c25ecbf3d64320705a18ccf40da75d64f876c79781b" },
                { "fr", "ef19d18aa504f3b33cbc7fa379f2c8e32d8c2ee187533c613efb05ac7eb018acf4e8ac416964e7ccb11d34054de663af1bf5d96dd20f48e5505d6c974a0e4ae8" },
                { "fy-NL", "49d1b869851fdc58e02ca7c2dfaaf8ba17feae8f0b28885d7ccf0c879357851c30ce2b41c5fce08c69a3e87a285aa94cde8a571126490e184b2765e8336bd11b" },
                { "ga-IE", "12f9116da64327a6b7b61de41b0370c13985eb1535ad3c8195be2ffe61b9965879c2b4aec0ffe08119a7f6503492f1a0be642cede88ac432ea813221fc1b98a0" },
                { "gd", "3fb94f0553d179f7ae5c7cc0bdcc6f482adc3d15873cb76ca71e63f3829e29b392a0145a3001c5b34b1a6dc085eed1ffb9d27228d265d4dc378bf77bc5b2ea5c" },
                { "gl", "cc62b8b376490772a898bb5dc8d06985ed0e0453e3c8ce39d7167fdf029df571c13797c949079e28cd655e9b2e0ceeefbc648716ad181ed7e95b129c39eb3096" },
                { "gn", "8be95a43915b3b4b1b311c2cd587c13fc636c2f70b4f590532bb47ec7f8f95d74a951258ac49db158c7d11628ea3ab56bc117716eb4197876e6724c36fcd530d" },
                { "gu-IN", "b2092c08ef96c8c3fa07ea7816f33dd9ba4f9b7ab2b61cf3711c86fcd2dde029d8503f63f3b53044b7ac858af97ba9ecad9880e000f0b5b99e2ff31b95b3e9d1" },
                { "he", "bb70778bd479cdd62d6c9aa64caac85417109de3f52e560940f835cbf36fb25ea03475c7bffded846f8d6c101aaa223b73bbac5eda410e04957002d5f5eeafcd" },
                { "hi-IN", "a4f9cff1e0a7a8f1411b739dd12b78cc0db892dbe6d0f0cb8657d51db132588112d28e24bf30c66252a792381051a62f47c266d0f025f55f255b44eb8214725a" },
                { "hr", "8efab6478550a718679ac4c80f430ec41d34506d286c1aaef17668c49466e7e9ded0e2efd14b48e8192110e8f82c4d64b78e49533ae2e426b3e70fc53e0b1e02" },
                { "hsb", "163383f7945a52492eab8c6418da036005bde489fe98ca85d638ffd50eea04c2bf003c1a9d9db4551021f5b9bac513c42cba8afb7ce10ebc18c8dc41e73bcf9c" },
                { "hu", "316d40cec59fb0efe21ef6f25b9c710c4722dc60991a668ce1cb2f2edc775c00ebd3872326f925ae0ac966bc9b34876d7c3276c7b8b7bb3b754fcc941dbbb34a" },
                { "hy-AM", "5d944a94b53cef6006da824c50e07bc58b73080076b47df849ce1c59c7b23011bccb3040b663040c89bf7405d7453cfdcfd8e2f8ab8442d3e3810e213055d715" },
                { "ia", "6f786dabc05a7b31ff7779fb114e2844f8f1d0e39a246cd59ccac71c4828475e8e91b2d8bbd9fa40305c4a17e8f8319b9d0c468e84b69a255fca5da93063bab7" },
                { "id", "054f73ee3884329dedbe663a3c12ef54c67b50f2614a5f889e2fa03181ef449929ced39012fff8035874af79c13d93b395d60737e37bcb17877d71db953d7070" },
                { "is", "2d12663e617f01416895899f083f83aea8e0155df904edc4800a7601502c927520122881c50e8c7496d698bf22d1d542adffda31a91d58a3c481f07f2e0abf99" },
                { "it", "91751ce68cad95e1235a40cf7b83c8db6d32c98053ae6ff6e5ed820954061ec1603e3613e547a786bbf3d35ed87f2d3fb582f2ee146fa178c04e795c50e59049" },
                { "ja", "c81bfcfa3c7ff79f7b45881d661baa160d22af8f583f6e764c53cf394b312933bdc412278095f8a8c45661d2bd68b85433a2293a2de1e8ee5ef32d4318b6392a" },
                { "ka", "49b41e4f6c36ed154ce82a8184f920e3ec1771dd5551ca4b7d59ca55ca5da8d4a0f342d913d639863f797615505e98735018ed7ce78d4b66337e2eac00b6c695" },
                { "kab", "53b0285317317b87d4e5d5b4ac8affc796d9d62ce0266bcc01fadc5d5dcbce4c647a5ad6a3879b9ee1ad96c809f406e92911603111850e113a168415c16c69fe" },
                { "kk", "2b13ac9b695b1866a4365c40291ffe85d88d5630dc64e1498ddba1d4e614286419a02624aacaa10522307cb43cde6e4814a4c669daad6ad4ba4e88308953da75" },
                { "km", "897a4bea353f8bd79b217828058f06fcd1711a25d4486b66e53f4c4aef6de1ce942b5dbba5bb8532a684bee010a11506e8176cadff15dcc4237d41860a50b747" },
                { "kn", "27f27275ece8e344709d3c3c1dcd5035d8ca85b002db58ed1cdd53867c0e7aec6db77790057588a2804907b251cb9e84fbe00b00e97a6f8fa08c4ad732e61682" },
                { "ko", "25e9e4739e31c7ef5322a248509b5fdb47b3c56e6e5f4ed458f93e433f0fbf6d3408798c63a4032a21f508f7c8ea1ca8588fef9b62072a1d493d9242f9b940e5" },
                { "lij", "f95ec76e6b6c27ed3ceedeb7b21adda0597e607ec9167d70a82cfc05b294ba47fab413c8da2df5431ad17ca5b18f5dbf3bf3eac9a79beba2d4be868ade2d3341" },
                { "lt", "823d35f1c5be533c4c3a205a83f9023f943f150c9a3071e2f624c1f0b5da1d48d2729faa7852ad652cc4ecc30a192f7b2aa12484da9fd54fce180a637f64ca38" },
                { "lv", "47d3077be7df1e1175a5993eb313196fc6476f367edeab9dab9a73874a7250b945ca6bc8d4652553b1a6389f199f2d6ff98b61f33af5d821fc843a02e6e7aeea" },
                { "mk", "8eaed4e25d17ddc9b4d28723b7025029d38b614099504eb6791b168ae5c5b7aab01aa4d92a56ddf14255aa8a4ddf834036f0c37b7a446fc5f27e2550dd914029" },
                { "mr", "cb89a6b4b29872e0717fc9ab9d5593934802a1a1889d4b4f62ca0b2f89250cda98a51663151933dc2b1a0f9ce3c67bc3f4576d28eb4fb783270b96a286bd28ae" },
                { "ms", "a36556786c61be8b8269973974b934fe99b88b45560d1d40a1937d4924774581f333b08b6f4ff13c7c522ea28aca197870605e195569587a928c89bbc8a2f11e" },
                { "my", "118e1b8b11081c38d6b1dc0d32005a4028af716d3fdc67140fe25093b695ccd73e3ee716d65fd5e8a0624e0f27449a9d947728205a43a9199a4ccdc33d6e2ad3" },
                { "nb-NO", "2b884d57c3064fe88443abbb90b51342a6cd5add4e4ce154453735973e17921bc119d53d383ab78b68e96029434e7560f279c77fecd4a2451f10861ce9cf2158" },
                { "ne-NP", "9211df9705eb8aa972d08e19badb5e6836d1c3e282fc510a8106e612d59fcfb7f4b034f3b4230f712dbf09d18eaa9d9e55621efa21da343ad1ef64524e5bb7f7" },
                { "nl", "15444a370a18d010309b0aa68aa31930eb3ac1464885973825f37e971c989ed8211e0392df0b6b6325589c3e537d84d2f428e056a248ddcb3df48da815983628" },
                { "nn-NO", "d85b38e2aa464bf788780935ac9ca906df4861296464a168c9ba53ec435f08d19afe9a86f58344d62f697558d96fe3ff74e8a61a9c7ff983a6e47ddabccc5231" },
                { "oc", "7d3f103750cd94329c054884d2ea9608013e5d48a65611e3d9eba1353306a71d826de7d55c14fd9b090e03a4a7f5cf6e8ef8a1476bd46f9b27bbb0920ba8c638" },
                { "pa-IN", "09bf0d11afb6facffaa9567598d35e689330d72aeed0af629d885495bd73319f19138d08b9776a53ef4df7acbe74ae8c2335405b659361954201b9f8fa799cd9" },
                { "pl", "fbf67e587d29e9bb2d6fc74de79bb057123b95a9461a11e163ddfbb293ac26377b6d8614fbd489a2cd469a88a9c3688eb09f90b638760ef7868006443f2de85b" },
                { "pt-BR", "5d0a3f234270cd635ce665b2e2fa903380e87ae89fd67c146586f25633be083466a353118b1222c237dd2b616e73dcf2c007bd070d59ff6a5b8f72ea33d468c0" },
                { "pt-PT", "b7437e8770cd6764dff049fb0af8b0ecdacef449e53e9b37251d52ff0a7772049a89544c00da787b01aba0d8ba41ae5efd2e9f4ce64c8e2450607ae6688ccb77" },
                { "rm", "1475bc2a527a8cacb7ebc3da04a897050e8e6154602c78703f32fe31291e4e0a50fe87674464af0fcd64182ef1ff7a30128fe0618b2dc279f442083d77a0cd29" },
                { "ro", "d9159c84987212042800cc4884a1215986c9fbe4f5e5eda10e02b1006f471967a236198a6ac18ff2993f8038fe1d3aa8c906198a2f7d93a800a9c957e5bf642c" },
                { "ru", "5e78dbe3026a0c62e52d8e3c8c4d71b24cc174359c803754cca99d869c7be67708a4cf0900823fea170011544a4deb8fc9f994ad2768f89891d05b00e914b5a3" },
                { "si", "2b9e7fa4f3a20512209120a88361519afcdddc3818cf8664d9496c9735810f397b5943cb53e1e5f84445024ececf2d7cbff3045257f1c954217e67c9dadd273b" },
                { "sk", "9364a5d5c6e919632e332448bdda269b1677440c4693fa303c72816e06655a6f2aff0bf16e8d44ad19cead267643d47946ea2ba159d716642b3aa235aab3627d" },
                { "sl", "d666a8cbb274ceb82a53243ab2d2b0aa1b711d201547c5965b4c8930088e4dbac26ec3dd9677d41077ced8da2eca26dcbb0d07436e0398fe804e1f2426aff7bf" },
                { "son", "4380cc4a1813f9eba1c77dd389edbe63b28a3ab1d4ea0f80300b45d625f62860e3080b769d0fb694ee22b50fdddf13744fcfcf7ce9a9b362ab3100ca0a8eb0bf" },
                { "sq", "75eb7ea827405868be9ba343aa72c011dc0c19c998f4e81818728ded1a4790e4ee68b00bc58e08e8eb5abfe2b97567171e84eaa17ff2f8c3ecd41e677137e49d" },
                { "sr", "45b0fdb7e1d4fb914f3a205ce53fbcdb812dcc029937781ddd9a69de7e0f5b8e1a5065ff64dcfb74b59d7d7b874141d179363b7b700e369cfa5e2469aab9ad5e" },
                { "sv-SE", "666bbe9c0cdba7cc26e4703d2771d2b805c382749c7bb048e7797ae663af7252e9e723fb3222e0b7bd6218ec9297cbb5b483886ea264a89b83bd6ff4422bb270" },
                { "szl", "20d7c8b022a7bd7a94078974a33e054ebe2de8957e370f7c4534a1f21757b241e8a8d3ed30add8c8c51e5f81dff286161460b2c5c78eb1f36c8fdc5fb56b488c" },
                { "ta", "23a6a82cdc797579a0f736f729348795f3e4e30cc3b6d3e16084b506a7557237463d46ad8c44bc85440196a25ac995570281a6c10859495862f40ce9fb22d0bf" },
                { "te", "0cc87426b237e4799835128d2c0e1b5d0e1b9f157674d17329ead6123046dd916a0bdbf9bef6a67dea6111dd1e9da39a75643f28e283d42ab6d36fd33ee59196" },
                { "th", "bd10d518461aeb178727311ad9673ed82290e8acfb267171c62f84073a77af498a9760bcd89be6a072a27d684809b69a8d97ed32473ff21d5f015509ce3e22bd" },
                { "tl", "88d261fe747ce0950b464e6bd7970e1945bfaaed45589e93539b8fce9f95bd4a6574f04a015f7b1371dd15d4210083f651e687ccde1bc5109082ce27106e542b" },
                { "tr", "340661e4fa65af99a64038272969f588487c25291466e76be89a34eff959fe237afb304180390c2de781dba46db8d8816e332fe8ef011463035d80ae0b54cb2a" },
                { "trs", "2e94d25f03b79c42f81a8512f4e4472f142fbffb602158b83601f97f1a021c4fc6eff3040e0762864b097616b86c7a3617ae86e5f0bb406ef8faead698ae203a" },
                { "uk", "d187362b0b6be5c4897443e51943bf2f30bf65c60b258e20d4b4b3619e15bd2e2b9ed227639ac7c27a74f424a2c84cd124472ad1f68decf713d853242846fa5c" },
                { "ur", "bbfe6a2a8eacccd4919ae4e7ae0257c165f1b97090c66f60562ae4b8ff2bf96681c4a65ea8d48ab3d1e21912a35e04c590c46db2e7e05cb7067301303f349485" },
                { "uz", "5251432d89e89e21678c28e1efcc809d3c58c0ab18164c46267df9339660c1e796ffaa47f6a97149e1a3ba8fd9801a56634ac9b3cc6a0a4955f5440ab2630831" },
                { "vi", "fd08c3461d7b781f69b700ffede1b1fc3847770fb67a58e9c3737a32e5e605cbb61ebcb987b685f75e4f1fa985a7566dc0222c0212e0ae260cd635050e421836" },
                { "xh", "53f897808e450f8a9e1032d88f18863a0903203b9d1422d97dbe34ddbb73c3493e0fef968b3c92413e1eb18edde3be8deba6aab2160c062a20a606fc31611210" },
                { "zh-CN", "425c5f8da106496fd9bc57aceaed8ef3991e6c703a64ac42c202248521ddf531fa3ddd9ebd7674d83a123678a93f658c7c9684a3d5bee5a50cf2710fff53e36f" },
                { "zh-TW", "160541c208a48ff5a6f175a949b4dfce2c17f864ed222094a50bae097b0083d5afe096c01a376bf59268b899f37c749b47be928be5c4ac1435c0e01cc866a76a" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/90.0b3/SHA512SUMS
            return new Dictionary<string, string>(96)
            {
                { "ach", "a1548f146699849dad5112444de87c5e8f319d6553c31e67d8f69d7ba2c01092c5898ece0a278d73e72cb18dc2dc367aae630e86ca6bf5acdf728d743e98861d" },
                { "af", "799f87f65ae094b61f1663c93a357060175cf4b5863e99ef9e24fbac54e5714a279b11f511c42b66c647ee6083e17d7018b9d465c65447b1afef9cac15399363" },
                { "an", "76080df1ded2c0f6a1c56735015f9e24c48b614d6bf243f20d4c1d820dcc9ac3ffdd5e3ee62e84c7b2938ae46c862297a1cde0f9f8c260ebd529451009f8d301" },
                { "ar", "aa7369a1d2e16235367854799a01e0472391c9ebbbd5a261e8224dc31f4697757a5eeda7cde061f551f07c82fc196acd6d63464db71b802e38d0acaed34a61fc" },
                { "ast", "5447be86f4e471a1f6a523835d904c4d1f44f46e7e850611f24077c4633509fce3270104f4a4a88ae3cc4c6c85ef3e967e87a1e2137f10bb7ea26918784fdb87" },
                { "az", "7d1c23db79406f7514c4aa755afd9e901a7540f302cefcf035455acabeb6b84c02b9d59d640e0fbe0c24bd2c3728fe54ee4cece71aaae7b44f3415435e639076" },
                { "be", "b15fb4ac1e7a12ac6064b735811f72ff2b293741c945b5bddde5cb62883c8e8bd397943055b9f92b57dcd86c439cddb052d5063b8d1eb69ec8b9727807f0eed7" },
                { "bg", "e98af99c72b47406671eae0c0a3daf143cae0532b4f92110a8b118261711d179ee4febb0913f4b42b8a192b8d8ab411a852af2dcbabe9c76e97828c19d9872eb" },
                { "bn", "e25dfb07318eb583ef58892f4a5df1df1a24d45c521542bca0a60d6b9d4825c3772470dd518529ae656f6a7f7de314833ba926374bc06153f65e57d126c39545" },
                { "br", "cb6ebe2f4b619010dd0a0945e803bfa4a8391c02a9701eeac882db6fae92dbc9abb7006deed654d6858e3b5354f9575411ad6272303b83240e369e207dee4708" },
                { "bs", "1cdfcffd1c9a076e3e2529b2fe6888db73cb92000aa01233a0c8031c3e58d0fa142d4e59fa17ee7cae018cb5522eae56269bf1220e7494835fdb1d26ced0129b" },
                { "ca", "dddfa3364ea5b98bf086092c357ffcb25f2cbe037dfb5a2393319d8f3be170d3fe4006b0a0edb05a0eac3dc741c9b7857ac0d43da7d96570e100de2b49dcef36" },
                { "cak", "b62b3ffc783329dac91f156e105b556f5cdf85e0429877e8b0760e5e9fa118ff463cd0cd7c49dc02c0d03178c57ee0d6c059255fd9b35174df0b796cb7b710cc" },
                { "cs", "1c21853d97fea8335a6b688e0c2c26b598da36edef00dcbea9e2958cdfff55a24f7976e12f4bef8a2f0b73b8bbc4f26242534a12258b3bf9fc320b1168cb3906" },
                { "cy", "130a2cf7479cf36ba3f5138e1c1e0d7e295d642ab698c8ad1894a440713f544098c708bae7043e80eaa08bac046e651a1e316186f2911b6daaa4b865a0988256" },
                { "da", "598285c426816a69185d7ada76cbd2a273bada677f40dcb968f2db7ef098d67698d984effe54aaf5ab025504cd0caa0cdf59e4f240dca73634e3ce1beeaae026" },
                { "de", "b816d29368e75555bac120dac6e571500f2b3fbe12c16473d373f4f1979dbacbc8df2e6402c885ca39cd021b343ee869918d17de2f3d2f5b1bcb360c420c1e36" },
                { "dsb", "663a14484e658e174e08a692790e3dc2d018202a7456f346b1b77c740471d2d336db24d48f3c4aadbae8508979be1890154c2f30e3a94fd9011584d4679f57e7" },
                { "el", "c408d91774e69dc4358dc3bb5c136546d15fa76e38e0f8df1a96ed8c5f65d0538a995313e0a95516e65bdc407a33381562dbe1f1edc3a83e841da5ed67f0954b" },
                { "en-CA", "8100ff19d807261e4dd0fa302001adc85c77d2baf5ad958f090569ae2687675b509a02e895bb1ba6003a7d8fa638df13e816b8dec388284d119ec432abe634a2" },
                { "en-GB", "50dfa41ecbedd9527520a8b982a29208dfaaa5637f3be4848078356b85f3379d023c28a04b0c84e86c1022feb016f19e2e63bc6d7380c6718c71f942bfd0cf04" },
                { "en-US", "07d615779786cd7c2b5e7e14ee8b5ebfef482fbef4cc730b733ac228d791df7eff203f0d24f51a891ea3e42eef33b905f62682ef75a6ad5aae7addd69ad31cbd" },
                { "eo", "4da876560822c8f49b00dee179c8437f8bec6061c8383ec05fe2829a0f35eb8e35a219a82cc6235da135309efd326ffae84fd18a1bfbbe8a51a84abac1f3d786" },
                { "es-AR", "b2fb7583acb18fbee48d00e216f1c7e3ce3302c17a1417800d909c9fac663605cce6dddf4fc527b7355c3d9019f37795f8bc23c283e258d856e725b6f1797f38" },
                { "es-CL", "4da596fd855e0969afe8bb53d2a575679157bfa0ff7147eb0b9c5f8a42110629d654316a2119763cb5c2fb8e9371307fc3165916be2b6f30a18650e2ec6ed445" },
                { "es-ES", "0eaf20b0c2b70891771669c00457161f4ddde2d1314dc0546a3036863ae5cd59fb95f8354492d9a5a06cc0228b39afbaf202ac45d90bc1180dbd39e9f9ed5485" },
                { "es-MX", "f3b94ab436b365b05acd644c3375eafca64677d6947dade85cfb0f635dc6f5fa1f97e17b23e0eb3595bd32344419abb28568b47a5a60e68d14207d57d6ef2358" },
                { "et", "7499427e018f5cb914ca457904435f00c725b38ad40e812ee50736214ea7bc29ebabdb176055dcc7afd1d82058705f7f592a472c965c3fd71817b24d01326a20" },
                { "eu", "9ac61f5426ff0c6e0108f7a4f7e93d052046693076308f2aec4b7270d2196f319c2c1ee37ddd7fee87329a9a75fc94e5a8545a8983abe19992ef9169f622bb74" },
                { "fa", "ef6236ff6cbac4ef2b1bf2c20c98701b4e3e0008cedef46f840ed706cd4257b1b1cb4428516029af6bccccfa7291bdf989ca164f6275cc034669ac878f26f1e3" },
                { "ff", "32f1e807aa053ae1a4b603a3efcf4e0cd662c67a72b00812a8d494c1c33606858e05b8d7125c38f009cc46cbdc7bc9a08a0cf25660808d57f49917aa3ab8d00c" },
                { "fi", "8c8ce620dafc737cb1c75699224e90a103ed50403e7b6d3618fae152bcdb3a2866172791c831beb4f5b6269563f7b5bb48df790067f025d5e393fd5227841429" },
                { "fr", "ca1a07c98d3442c01173967f486286544cc5a4a6efc356781cc2bc8665626929e535118730060553aaecac53b3dd5c70f42fdfa984cacde5ad0d8db8758d20a2" },
                { "fy-NL", "caadd828568ee8879579d4b6d2b745327d408d88d0450022a4f72d3c74e4d90e643bfca541381b0e29752b13d0d4baf01ee7a0824111c9be4599fa4430f31b56" },
                { "ga-IE", "503fe20151f43a9190a661019125cee65309b43f5829afd7c3183aee0496f8aa078cac5c020d739fb1d51ec0e8736844b5607744e6f366a8082e455be1044164" },
                { "gd", "3c6d2ddde70d02d24a1ef613c1dc07d19b98910ad7c13293f687e06f1470c50c641e2e0744068e5eb89a3273c5cc0d2f78b950d2f4bfa6f5607160e588f72f5d" },
                { "gl", "f366d40d6ba7dc807f20b3fb76a618950c1e0f0fc231db2affcb749cff536668265fe48b4019afb07d385d80d49e61485760e80a724054765180fa35641f2fa4" },
                { "gn", "06db6deb1a30149f6b2f92a766d69bf0a1edf67262a5ce593d001f75809e8e559ba36b77383937f86e419e1150579c17c705689e146dba6454a4f32f8df98a26" },
                { "gu-IN", "932dfe43678f202aa0f0a044a907ae1ba761a766063df886c61faa76ca4a50c76e28e1afbb5ef0e0bec1842952e47a6a98e11013b5838def37d26cdc51278590" },
                { "he", "fc418af22986fa748be169ac5ceb132b37ecf6546bf007857321e04be89b41275d8fb5f50c42807b5555dc6ccda204e4e04e5021043028a6f827bf185930618e" },
                { "hi-IN", "19c6d7f338a3fe3c0ca6eff884acb47a9db93acd3b1f8f023534d982d199106fc73449e451b8391539ab023bf795f719d59b72926e48773244570746d8054319" },
                { "hr", "db8aa3023c0353b960cf81fd4bd38a935b6deeb89c9b784dc95e7b3a70600f9c6a0f6b6466a6c4082ae80435b33ac4aa4ba275c98ad26f126f0afbfbfddf521a" },
                { "hsb", "bfcaa54109251a7ff8ca53d68be8a9d36c493c78a65a763f8b836b655bd49c9b522ab42b4ad1a323fbe06259dc3fecd1ee14be94b0aaf6c30274761ca918b503" },
                { "hu", "3b9affb3e284c35031b25b7bd4d5c52abef16b63ecded9099277c45fd43cb85d97120ab2b005a731609b9cdc9db0b55ae4a9d1f3c3500c7e2ee6abc74dc3b184" },
                { "hy-AM", "3fd7e1f3ac00705b273a161fa398c092f7c068c054fd6ad03bd7cedc9ba6f80f75d8ec7eb2a01afc703b5a843c8d9afef2f127fc43b4c49518a99ea7ad94ac49" },
                { "ia", "3e807f7989445f0f999e0ead4db9ab16543a8aa7c8dd419794a0972ae4f8a56a8b297d4406c819fc4448318af720f3c1d24108a8f7ff2b2c8aabe87bf625efe5" },
                { "id", "3bc366d7dc42f90c87d14aa50cb7c0e824816e6ca794cbd89eef6e2749781397eb1fc3b2109160e0c80f33cbecd14cb97af2d8cf6f5be59e2f00bacbbe454452" },
                { "is", "12ebcb3d0d7b116b158258af88f3238c4401497168e791d1a16b08b48dcc75e2c586b09ad2de4cb11c4c2f41031c4102695a784c4506ea034d7302572278dd83" },
                { "it", "7e255abfb34542a64947b57427a79b3ead3ee8f39615207d49d22c6ad4db29c263aef25954b058600d5b85ab8cc42819c4947c942652da4850ad5aa06836e411" },
                { "ja", "ba059e56327948a33a6cf303cf47ec6dca69db5ddd7f4b79ee5c3fdb1be79b2e6e626dd785fd759e1830605a2a9e94ac5eef80bb971443239d2ee9baded0622b" },
                { "ka", "523f58b49fddf6b761a84949f21e14f88991974a5e43b94d5b04ac8385cdfc0fc7fa166ba0015e0a1743d8b0d2b9312ccf2e029c3751cf612e3e6b54f89998ae" },
                { "kab", "5edca73045feb9321c27a7df1a3f5962472acc20e97755eceb5eeb3d184f0baab4b5f5712c651471058877ffe69162e07f8cf72f9fd01cd718786421132278d0" },
                { "kk", "115d0bd72b20a4804401a0b2fb9b5efcad469f542438f34b64b56a9ae34708a76a6e8dd7be498326d92cc507e70de6aea52e50e1bdc7d08958cc237ec981f652" },
                { "km", "c712fa335c2b3557b5e9b65235d85e15e3f551b42afc6e5122dcdb44e66ed4ee512b574e4e0d61b8f678d87485368abe1b464a724272478d724c3e6352e5d7ba" },
                { "kn", "578651169cfa313fb2278aec45e97bd30330b5dd9c1c9f942492513b6316d8c8f84f0acd1590c71723b95f4b84d1be443fffd762869b645f553ee351d7f00cd9" },
                { "ko", "99dc9e704070d0ed483961b0b4093adc69a5fda410005d2dcf147a0d3255eda675156d3ee3b19fdc1e475f00bd4fc05078be8bd9e54eee037d48c8648ade03e4" },
                { "lij", "428a74dae05987d186ab20e5f06c353d9d111a63dec7a20486a01780bd7df149144d65dd93f54f0ca727c144ad39851919a8ea885236688f30f7b4a44b74b53e" },
                { "lt", "9ee092ebefc66a9a943a9ef8f21bc67503e0d019f67d87512e15764beeaee9cc187a6c3dbd7fccf8d045d3f9c1ce53d3ce95fdaafaab1de9662e34beafb2b551" },
                { "lv", "781a3b703e432c0766ec81d91d07f92700d275f14cac4156161a55170213631652565aed76848b02e780d4362507a691644a8b8b3c52143e5f0223372c9d79db" },
                { "mk", "d569762fde2cef86bc9231c0f1be18bce7640845d7375411ff48d59da4c1f891bd80be15256a5ee8d8303072cf305bfc35e7a3312c1c3f1fea53de9d5ec588e7" },
                { "mr", "e3b372d15d8176b29b7b14ebb925eeac288d41b3c7b214e60a7d85f3dae2668814a1c95c12117afccfb94bbe8f0f79b1eb6182f02f1d58d19daecce4b673fb86" },
                { "ms", "45f06575eb945a5c40b9dbfdf0a0611ead09fb1186cee48f008d85f6e9f09c8c5e63c0e9375e118b974acc03a24c3065b474ecadc479c9fc149d0a0f0252b4c3" },
                { "my", "9aa038377e448a52bb182a2c70e2979f0581bc61fec41581f21fc05f8ff350ac76291ce79c9a4111a8e85a973a5140842224f52b9327cc257d8f51777511c59f" },
                { "nb-NO", "f8ee61180b67152a9c7a125e429ee067f169c3331b259036fb636c3daf4eeee03886848eb5f0004f12f666e7e9fcc83c4de353e3e566a739862872f23ab446d1" },
                { "ne-NP", "eaf7ccd819bd89b1ea5927f6cf14fae9c16542738d98ad610b997703148d4b6a8f178b56d84e0839ddb0f31cc11fa1800b0e2aa1b91c9dd7b222c69d50345b34" },
                { "nl", "5c3e0268c7f1e8726b5cae577180f1b2be7be6bdb0bbc3789381a01f0fb86e03f93513c605252d7c4aa04d4800150d56c696668b82bdcaeb8437732674159c5e" },
                { "nn-NO", "d0a8b9b81405ffabfc06e6e4987a25fd2ef3374fc2a751bfa1aa3740e8e5d504284c009fd47fdafcabc594fb8ea8243bd6a9110b72f778154c85f9f4264e884b" },
                { "oc", "49cb0677ea524dd023cea95fec76b1691971d9ff576ab89568b6fa7b80495ee77fea739434ce21e737edffa9d755d64903f7f1252fe7b39dd9585568fedc6a9a" },
                { "pa-IN", "e9920ebd2549c1b5b882d0b01f80ebba588d62d6393bfd09068392f698f84835b343b4f5a594892fc032c897f8b94876c6df935f1fe833b4fc527246ff88ecdc" },
                { "pl", "b69cda98345c5b23ed4df0363d714d2e7409a6ce0cf8e10de9cd9c010f039520d5c195037bfdd96f72330bd2bd02b863d31e8dd9ced0da2c48776874242751af" },
                { "pt-BR", "ad22f181892d30226f049951806500f829302ee9a05f93c7e932cc31b7d4f29723783edaa40723831d30d668ad9964978108049d8d76956cc7efd1f0890e37fd" },
                { "pt-PT", "708d22b04a100a26ada613f27e6f49afe704fa364a1673408037000d6575496309216767b31912bb6c4dbabcfeddd8776f5bb4e973375f4d74d90553b4fe63aa" },
                { "rm", "bf8c5c3de5808c5c442e2fa270587b24ef0d01f3539f9f764e5adff59066b839242c78c5da55e521fbae3f32454d5b3e92e4dd9a85c89dfd29b26c780aed59b5" },
                { "ro", "f233919ad6318764c5fc9b9884b2f43de02590ab360bdf85d09fcf96a9d7e3ede3b854455639cd77a967a50ae617928327c0e4321f361d06a6066c1d176827e6" },
                { "ru", "aeb8b8c82a68d975b5825fab8b52eb2d6959d2ac8156beb1f9907407a7757e3843cf0799dd2aa1256fb61dd0169c99a430859d41441eee88d0ba9a9c9b55fa52" },
                { "si", "5f2f2e87fd42b509839e1c2347585e555eeb32897db7801244e78cb5048f2d4e90e225285797851e28de2aa9edfaeb8a47a5751f85a708685a43bded931312b7" },
                { "sk", "b80d8d1e035ff373540bc2bee4565bba354f03a93f079395f32cd4a7240612ea4d390fb7dc6093f5dbd63801a005b3f43a3bd349bc26b6d45f78f73e1b3252f3" },
                { "sl", "bd378699bc131cda14a4bf3c904bd4f97bc649c83a95e8f5b32b561992f694c82a4aa27806e809c31f9064fca8a012530ad5a9dec7d927cde4de003aa4347571" },
                { "son", "0cf56e81ed52007abebbfc10bacf8acdda9411fe8c769579bd4bf79b47770066c4f4be8745f8b5a3b74efa0485540063ac2c6631c3f599b5e3f87f1f33abd179" },
                { "sq", "42a47381287f7c8b86a0e1cabb83a04c384d6f98aa6f896100b5f1bf25ca2c67a165e1dd0f52ca416b97a3791c34a80e0bdd59e01102f17b984eae8b7e2061e6" },
                { "sr", "da973f2ee3efb02ef9818ec43a031666a6afdc4d1d3070612b41968e0e0997a664734efaa36c5bf0d129f9af0e8b47a59b875437f337c339a4c14df664c29efe" },
                { "sv-SE", "6d2e0955fa9581b6eae4402acbfa69b868e4eace00c2b0051b2afb14e92379915d4dcbb994ebf3c3efccce3381c45c34c861cf9148bf77e5100af1a33ca92cd3" },
                { "szl", "b2d8f832a7cd00862e4471aca55bf38b16ca850b0ab1fc8a99766d3b04ce5702e7dc41919db56c5d3ac1fae19807a172c6dd86ef507a46808b2718865df1ad0c" },
                { "ta", "2d57f814c6ab45beac6a9681c0d8e2c23e513635239c2ad8004b983f63cc395166edb3595b4cc7651b30846c847daa01f08c1c492267e0d349d5f3f2c59a6115" },
                { "te", "41b5ecb09299a6e41c5b9c3680430d1932b130799a8ac584484e32e343bfc7a66b13e6906d17c516a6f9ad672d2d3c7f59ba27eddade66446d2e5dbb29ba73fb" },
                { "th", "d03756cbad8b7d0959a263fbfc144b3051f6d546596011042c6f648f5615788f55a2600575f60c577a6ca4a7c1a2b64e158d66093a5127be67124ea04ba48300" },
                { "tl", "7a17729675e9cde578c9dca1b6620b26b18cc1811b1548d18b6540852cc7fb66f3b649a6884677d7090cfee73b7edf973c2fb917f3aaa87219a435ce1b170803" },
                { "tr", "fab1f1fe5aa213b52b358b744bb1346e9aa98c0a5152041137ccbbd023e2a965faac129c73e15a957ece24adc9df2b414cd0c3de9587728a875369bcec8f7b57" },
                { "trs", "85a689cee2f0c649752d7850a3a2d182b8c8ccbdb5019fb14709b083aaf187d879792a55d24eeddec15945b20e8c4b9db1dcb6a7de6b0f8aa1816ea81f12a757" },
                { "uk", "061f8a01edb883a2221c7a825591a1352646c9c735e6beb2ab0b97625431ee2b90f7e2fb0e79e6cb424e2b337743e5fb6900a7cf969edfc4007f4f9a4dfcd2b8" },
                { "ur", "7f74011b79940853b0da30da177089c98ad990dd6c0ee4fd646d7b6db68aff6f8aad44c9db98fb7884a36caa5bb800358b19adbc1802b907946fa76516af8da2" },
                { "uz", "a4bc9c79f19ac466e88d5510cd9bfac0d3336c7732379093c0c225f5c44446fe84082e1a2b74237ba7a803a9b51950178e7bb837c8a3af15a9f019be38b3fc75" },
                { "vi", "0e022f59c09a99a693e6a57dfcf720deb693697a535beae159b77f32961ba3da4eaa872c63c1fe56d1054248638434a9d5c344fc8abf212026bc98850571276d" },
                { "xh", "fbe0083f54d16e7a6755640a3b9b26bef240801bb71b083c0e19019f81bb36bba6f62637c1a03f765abbfffd28033496b6f055056ea98d1a0145cb8999d3c04c" },
                { "zh-CN", "41a4d42f136a91f3c5c8f71ed9ecfab0554d001fd6b7db56b410508529b1bf07566a68e7fcb997baef41ec8c9fb97e011ac65fb6c9471b11b7ae015287d696c3" },
                { "zh-TW", "d8cfb9d35dc95618198feea9c39ac6aea70b74328ab26aa2f6598d1e1093f542b8db261b42a15fe9485b8a3ed140059d3c1c23924fce35b864d32291fee6d4eb" }
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
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition [0-9]{2}\\.[0-9]([a-z][0-9])? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition [0-9]{2}\\.[0-9]([a-z][0-9])? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    Signature.None,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    Signature.None,
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
            using (var client = new WebClient())
            {
                try
                {
                    htmlContent = client.DownloadString(url);
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
            List<QuartetAurora> versions = new List<QuartetAurora>();
            Regex regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
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
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successfull.
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
                using (var client = new WebClient())
                {
                    try
                    {
                        sha512SumsContent = client.DownloadString(url);
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
                Regex reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
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
                    Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
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
                    Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
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
            logger.Debug("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
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
