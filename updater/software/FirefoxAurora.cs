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
        private const string currentVersion = "136.0b9";


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
            // https://ftp.mozilla.org/pub/devedition/releases/136.0b9/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "10ae3f773c06100a8cef53c271ef8813bde847f27c6f215e03f8761db7642b1649be6781d793c44f1a3a3eedc62edf7cb10d064131654be354e263e2720b5c1d" },
                { "af", "249e0f81072926acf7fa877d43cbb23eea870bf742fcbbf2df921e0cf552e0dec68c252265488da70f07c3b81582263096420a78534e4377048a0863281ae1b8" },
                { "an", "a2d913e88e1e5538a3a6c025e30a68c7d090ffd25eb738406667f81e20224be98ed01e81d32f89792ce1f2dd9502af04e8828923cb30351b7f357432da059855" },
                { "ar", "982c2e80010d9c2f64418080cdd4b1d772cf62899474f367b72710a6e93d6be6e018e48789149d27ddf70c28abfbbda9c8f084bf667dd4bc30d35cee576c7808" },
                { "ast", "87f48188523689e003d39877fc8faea1e16aaa90620c9e939b37c893246ea56ebd2efd5efe102e1999f124a15d9f23284331a5938e11002864ab571b3a76adc5" },
                { "az", "00c26a2b34dc7eb2c1e12cde4c50cce8b9ce63bf478dc0d265a288902a6763345590df9782edd438360d6593cb427140650a4fd59e2981a3075b608680ed1284" },
                { "be", "55150e8c216bac1dbb0c1adb78bab227c07e6ba276991e2797e28bfd8cd8bef43f899d71f4b2f27a749d38ced506d7a454a33c80ca1589054afe989f700aa33a" },
                { "bg", "5eab4058749ec8f560186dcf64f44a74ee92c878501c2c358f9fd5e56cc97045ac8719a101d076be6b428f1ef1c8c57f6686abb1d88653b756517b84bc823dcd" },
                { "bn", "a078f553b54f983774bd5884286264ea78a7440fa47e981d041dc5660303405cd9f3c277a0ff16b6ae8211e6f750be7aef7c5c21ceb8d192c280a407f0f19366" },
                { "br", "fe45a15bb365006e4c71a55bb12208dacee185dadd95627d593d0249ee59d83c0b4b70257c1c1d91cc99b486dbb7e3c264e86798112f0eac50764a8f4cbfa8df" },
                { "bs", "85b59c25cf1ac295b4688a27c61883e633195556a6e478816fffbceb11ee3225cdfe8d0fa7974d19777c5e2553618f13c873a2db4d2fa4ba462c70178baaa444" },
                { "ca", "97c925ae7a015e84dbd84445518f34de93466f564af458d64a416099fd4908177b6924685cf02b53995397336863b405cfcb2e4db31433beccefad636dad5af9" },
                { "cak", "0cb2e81becd8c58c02f08a24de6885a9b83d100dfe5f04170ec1800f238c3b667c72c3e345a752db93ea9eb533d530e64513f4b8546887e2043f32821e5db208" },
                { "cs", "0e9a297a13a7dd6f60c8a72a33008226d2cf5867f1ca4ef22b19c79c823b2b7035e7d2e3171f25bf664fca8af12a2ade3f1fd11201eda91e86b468ec00d9e79e" },
                { "cy", "54ee477dd259075956cf8240d3d8f95f650adab45b96241aef3a15d68cae411707cae63e7ca3d3a6aa8f626ff70e6852044ace5e6d9a911ada7e23946a868142" },
                { "da", "cc3ed9739939d17ffcf866cd61c63078631973f04dbfb4a81185b15ca2aadb5564f8f452d88f30288caf109f2e35859549a85f100dbd67d27e923a7b1b30784a" },
                { "de", "15c7e59868e317280e472b078ddd47da0e068e2b38b6efa2ba5d1a7d97d099a13eb251eacb9f407f487bb2b3c837a073b324b9c8b2330c6aeb8b470e2e75bfae" },
                { "dsb", "9f380cf44846727272869f236f3d4096f780136853c01b1c3525bc1beb9b25ae30a66e9b3c8f08a4f470c228be88f12d2d73f7ad92943bc6b381f56eb96b9e1b" },
                { "el", "ce4e6e1b8e76a6e5332bad92ef27660414f42b4a1dfee2c663818c7e75e12d1a086a198f7886bd5975fc635806dc1b10d090475d0eb3ff26033c97368bdf3202" },
                { "en-CA", "7e295c6f9e651b4a867e5589f58e61d289dffb64975084b129d9fed6e7a169639b8174be2d65106008a450cbd276eb199bae2e71c3ad34e2e2824406b737a61b" },
                { "en-GB", "21fe9b9ae9c8cfe29db6372c601043f2727a95ea4165430b738edef62c8861cc180eed46af9f7d9bbbb69173b4d730f543da61b638b4e8013db738445f9dccc6" },
                { "en-US", "f7b337303a44de35fd2a6384d050bc11a486009a0d31854a5654ba13e3105c2314da7808e7759bb8e28d4d08148854249f1272519d7880a67c31dfd0f73314fb" },
                { "eo", "9c3b9a96cc6b7884d4111674b83ee02470d35c7b9921cee131ba22654651242e981a20b59b9a807fc8a6f06301d3a649af3388374bc2879454efe2782d8c34c2" },
                { "es-AR", "6ea310fbf9d1a7782aa84f214129ef9e53b0ff138b573d0b0e8e603deb32e2c6c38797cbc79ac0bd3fbf0094ea7fffaa8a1b29c53683d146e38710eb5e196e17" },
                { "es-CL", "23b256b722a8dbe5aada8863465ab8bd4738c85b9e120bf8cba677fc96656b1778c69a091b19cd7d0057ba6b99ab24db933ed850cd81fb112cdde3a770cb6ddb" },
                { "es-ES", "b13f22c8f93e429639256dc4049c8ba2e27e4b053c273009a6a4db8478e71162b50ddc89a78f227e36784fbef11d5143604eab420b3427af5a1216aee9edb0d8" },
                { "es-MX", "854ec00ec766029aeae6cd7118b77d2aa3e0dda53c2baba5d16afdfce29b7d95d42c87aef3b7321446bb86c5d0dc1449151772afec4b61d0fd58b1b958152dea" },
                { "et", "e44bb435d5db6779f70d66d997723c0e704d25394797f1b17091bf966056d66818e551de4037f42113785b3b7a493ca6ea3fe401f2736e9875c7d7e6274da034" },
                { "eu", "5826a5ce76178b59886b964478194df3f4c795ad6f35df5a4085c234c83861b419862812da2798c4eed8fb78a23664da1737b2e5403a22d00bd2b1bfcea151ea" },
                { "fa", "3320570f4d00596d4b38d04cc12aaa0e2b5433160e25af2fdb36eb054b81b29ada9291fc3015aecb6f897354348e7630b3fe2d2a7757da837839c601b3274789" },
                { "ff", "6c4018b41d26adeb4cc4ced7a5dbae9373a71c4ded4473035b5a12c0fff1b4380c5b56f4862a18ac9bcde91a2869ee8c281db86639e505ed47268d39eab85e3f" },
                { "fi", "9070276f4b836854ba158936846821b2b05a5777bec778dd08e3163bfddb5956ba7d5bed513b8bb8f58b172ff131977332f61d594967b7f981598b43690d2adc" },
                { "fr", "db9205bd746eda1276051b4bb0db4b952cc6489efc0d93c842a64736f32ac0d933ba27e5964a00d1654cb1fdb39a0cf64acc9420674d80a7e75790f0f8217761" },
                { "fur", "ffab425e4eafde7b6c7fb3e1cd432791c57eb427cf4f205effda77315e895fddc62afc4c6e347c2d0bb9307397ef09914f843efbfd60f273450b7b082197729a" },
                { "fy-NL", "52520700fa2e38aec7c6049f511d7652e1aa580760cc2a54a40c034749e09430ebcfc9127ff79bcb1ca5430d371678da494eee2e56fadf8a9a08b25984cb8423" },
                { "ga-IE", "6edde79a7dc8ce9a5b0546e31d50455050a1aadb035d282cfabce0779ddc04f0a2bf9de745c78a84bbf8fc77743c84b4e267bf7ce23d9c5bc0ed6a918fa00e06" },
                { "gd", "3ce56643f9809f5818aa8edc3c1c08d7d14084c72884e71fac9485caf483e2a48d36c49daa4436ba7d3f4980f3648fd4e99099dfde4396937da1ab5d4e1039cd" },
                { "gl", "c785057ea23397323bd87abc9a37f1e68a5399a5c84cd706a6b9073c37d69303669914c66784320e68e57d0a463f5d86a0a6c591a2d8bd4a2bf582f62cb61ab0" },
                { "gn", "c1c2eabe93a74bae8570043894b854d6bd3aeaf21435dca3fca68104bad3ceb393eb30e8a38e6514c0457d818c45d13d7544914f6465a8b0e45d318051882f73" },
                { "gu-IN", "e723c82f851d97530a9e2eaa9faaf87c70b2f556113bce96c30d65246caaf3a3597c7d4a9006a31dbf812a703d421dc8cb0c8203fd3dae590d837df9fc8692d3" },
                { "he", "d362df6dc9a542930d698b77bc70534407f8fa9f2d1ad67f47bdf9b12c24d6c2eb6cf1e1b146ac4eebb4b68242cf1d5ff0a0aa665c780b3027496fbc84a4d333" },
                { "hi-IN", "58a9cb00cade6ed4f84364193f6a25964a95657e908c7dd531b9e6eb04c8f5e565615e7aafd12199e8e823751fcda8896a808fd0a918b4bace8d349aee851d3c" },
                { "hr", "a63c01d1039dd527afa4fc44d31b1ec95deea246b255d84098380aec6c6769ba296c7cef689186c93ca09152fc71632fc884a8c8491a933c05099d03354a198c" },
                { "hsb", "8ded1b0c02c23db6b0965cd378b16192e766959b2b61c7e58a2469c7dce01e2adadd9e18cbfb588064e651abcdb61b54e35a9591b80b6cbe99438d528fff3eab" },
                { "hu", "8a00e662d3623377253b233d75ed651e9634d291f2cbabfbb1165b0963aa94619f145adbc4bb4aee56c4756231fbee58fb1b5f740da549e9c9822f3e742b7f1f" },
                { "hy-AM", "06b76c2693d79ce703b315ac7e6ddf5c86900423695b3666eea6029540eb1cacb3041caf04a12b5ba3a7539a3207774452405f182d501e0f3c9539a1187d5dbb" },
                { "ia", "bb6edf48a499a10aeae67460bda992802d839091e8dd62ba20880f13ae8281e9527d3218bbe2922837f4caa9349aabf0b43a75499e393db59a0c42826d8e7d7a" },
                { "id", "e9d5c7b368be06ee44432802da2a7687bb630a4e49cd04da265417594b849942a630e07185822267a4e4c026dbac6d7a0243f79282ad7c607e734f6b16f61e49" },
                { "is", "9e155813d905a0fd0ac7af088e167b654b76e0a1603487c2e9c2e4f548dbec7aa719c0b6b028fa63c4d062b3a827111dd3d65fb582ca4bea5f243ffef5c5f3d2" },
                { "it", "e9b878cd33775297afc3c6e6a8a33085c16f346b592c18f2d45cb1bcd28f50d0bfbb1f3806d0386b44a35f0c420df730fdc132ec2d50eb5a4136559c2ef6bfca" },
                { "ja", "719638e2b8beda0cee7fa46ebb7a02ed5cb397e9073edc936aef7479a5e50b7c6bf2baf479b3b8d11daba5945b597476b8f34e0d84675bba44c8cee507abf083" },
                { "ka", "fa1408a34c48c70bb801068f0a98cf09c90b631faa094e61d599c8b83fea036990ffdfd1f572fae2e59bbf73496f9e839b25a5df6b5da92f7d451e0c94c66e7f" },
                { "kab", "6c4a0b29f9f18490ff3f969983bea5a98678805d4a132cf41a89bb085edf969fe3b27d31600ce3b55487b5f434a786a12d6f8fd8d7f370ab9fd0e2c4cb42d4d1" },
                { "kk", "6486ef6733ddb45c024e2991d28e3db81e4ba0f992e861f81f20795b61159775460460c64a439658bb4a1dbba040f9e25d9c32981a63e202777d0c272b909d27" },
                { "km", "2cf628e8eddc180d7727cb72b3ced6246ced823d29ee1338d8da8c40650487715bbdbe63a269a7d5c6799be50bd9ed69309110ee9ce54099bc262f65177378fd" },
                { "kn", "c97e59313cf75c28a5afec521981fbeab99015c8111196c911ce21fd60916c0c7aabbf6d204d2baefb54b8b50f8979c12ba81770791da7cda277d3ef17ef847c" },
                { "ko", "224477087ed1d639f07f00cbd418fb78d4e5efc789371b8018420cdfe91928ed6e1b1d10290b6278b4eeb29115794bd0e3784270696239f87776273d6a7f7940" },
                { "lij", "9cf939df6d883f8fd56b167b939f5561c11dabd2ade9f6dc1a08713d0ae8f3d58ebd450b3bed5f16c3818ee104e8f213ad5c3700d92edb442331182c29fbae5f" },
                { "lt", "7cb3d41e25ff36f7c87d8c5abe0b02c4fdaa433e332e9e8da46a84ec6ff0ba10dc154d2f0f708a36c6df6c5cfffd0dbc2d34fac84838abc0889e8282db61637d" },
                { "lv", "80e9b4df6750f60dba5d577d45f6dcca2d33bcf6add487f421d35dd5643a75958ee4369fc0a25fd22fd997544dc8690bc0d4798b62dc4f06796aa25b696aa703" },
                { "mk", "1aa745d4866f071902f6207b64592fde8480621825ebc0f72567706dcc57187aca0535ffa006fb8329f901827c9b86f7da117ea71ab1b8ea29aadf19edf340c9" },
                { "mr", "c62551bda678556659aa43c552b54476e583c863cabbeba91af7bc16865b2cf10fdf591debb1f0cac7d92a9969c7254f7bab774fec29bc25350b418b0e7d4523" },
                { "ms", "7859f37aace6c74d90c0888b3b3bbeffd0542d59f24ffb00e77742851b70795e741d14a5d44ef7207bfab82243abd51ff947f637219c8264879017248f6e18da" },
                { "my", "b18576b9e870088d56de9bd1125e0a5f5ffb422d7334fda955982554d1efd575d901fa4d46ccf457180fcc7a94a04616798bd2b2ad30928c5db5bb52f0c2f222" },
                { "nb-NO", "4e3e04cf4dc990a9bf5001edc63ababec4ac912485e1806b49d306b9533ef510fc895c6330db08c651a0398c1165afc154a976ebcc9839b146a3e22681bfa8bc" },
                { "ne-NP", "ec70cd4644ec2fd5b10c8980897ec3f6441457662d4a6381076d1d267ca2e330ed4d234b555b858f837217a1ffd49744fc2a77a18cc95bde21af97e5ab145e8d" },
                { "nl", "504b6d8b7c78fc5ca84cb44edf19b91106f9f490d553016d599808ae4828798eafc20fc15c27441878bda829778b7f3c11cadbe4461d1cd29c35b46c8ac096ea" },
                { "nn-NO", "596f1137edefb8141440008bfc632d99a5594ffdec744de0eebe22d2a4e2fb716589d3df6c8b58f1a45eb51168fda631e7df0f3d590b7dcf94bebc3bcc723f58" },
                { "oc", "3c7b7933f7d649619d1c8667f6a5b687730e433ef6b3e48d3ee2672948957e5b0e4f26b417d0dc681e6516109ca27eef728c86ce994630ca0f6aac9ea4c2e533" },
                { "pa-IN", "8e02e41620c178c55a56a7ca808f80ee19686ada2d53c630192c2b1cc76d64222696dfdaf13ac5f3bdb09a810b69f1455c286d038be898246d2c3bd212d2e4ab" },
                { "pl", "f3d6da63a6d99aef7d0e22d525ab61de802523c18ffef381199bd394e529a89dfff0a8389aead90f49d6514f575f698ac9d82bec1eedea5958174b574e232a34" },
                { "pt-BR", "924f0358c69a318d257954a738885976e5e9e15999d06bdacd2aa2a5c49a31018d396acf41838f28e0f0886a6cc81fbf77170270e75701748d7c617d75c5a8db" },
                { "pt-PT", "c336a74f54de9eb2296844fceb5e304e3476e3f6a04b335e6e2c7488390fa317d1826dc38400b9d5aeed3f8e8155c95fda06dee8b16421d7c1acd5d38e09716e" },
                { "rm", "76f2a362ab6c6b8922921bee01b15d7b1a72368bb490ea22ee59fe414e1844f4bc92f77adae8c27ec9b8fbe9b275a61fe4d801d60fc7400ecdf4c6f010536a85" },
                { "ro", "28f5e8ff42081518873a8894ea5a71ed09b558a3387f9de33195ceda1a93acad22e1862d460acb3adce858e6da26e231de215e558b74f27dea9d2adac5301a81" },
                { "ru", "eee92c5ba2cfb2121bb2c84a7509e237e06d86f6a4b21761442cd88de867de6db9352d72203859904e71e279db9ea41082bf5ad30c1e09b0034a54861c1f2a2f" },
                { "sat", "f30a05c0fe9b002d39de2853a4c3958f5cabe7009d2c1b13bd82a8f0599a0a2e2a445bce275721a03cd877d9c6dabb2c8818903e941b89eb8fdb54cdc2018ed7" },
                { "sc", "869321101e6e42ba7a46dec9ffe513abc813d7339f15f75dfb71774fec419d7b0452016abe8597fae450792d184ca683d8b87a33af00ac824480a4a101fd1f9c" },
                { "sco", "3d63ad79a2999c7fbd402f5a21e7922f66060f11b1e6c4737b908abe8c538d9da7781973d4b99d48b7e4f9ef20dedbb489da02ead0c0a8fb471ab8385f6b272a" },
                { "si", "d216bb9aefa65f3d99ee1dc27314fe0a829612ee67f5579c17ba753bf0dff56566d2551389d33d8c39143d89dd7dbb3a28f3a643bd35e17a016b939c4f53298f" },
                { "sk", "e67ce3ae44064b5fcb9f3cce27dabe0efc4ac557aca468b0e10fde0d334a141042efc7d7215a80d5991a5f9f09992f5af7db1941fbb703c44aa652993d5699e1" },
                { "skr", "4dad9a8665cff241f5b41a5fb3a81cc91cd046fd30e397c88517efa2d41fa40ae3d5e371ef2d6885a4f9c9f36a2f0cc30e0d90d624da2531804893d272d2e031" },
                { "sl", "f10f5b0c9e755b501bd59b67a851bac2cf3f32863f011be405e585a329deeefafda93b3f15a3a4e11fc3007112cb2687c90cfc832cfb46aec9dbf3a24b83bdef" },
                { "son", "8df089e39fcb8379eead272f85ac746b9ef1ef1b60054aeb35d14fea05ad070b1684e3b86d3c0f405f30ae0e65f08f38f9ab3d41678af90ff9de7477c66aee01" },
                { "sq", "e0f7a5c6247d518b26a35596c0d94e462d8940e2105068fd668336e8c6e200ad2ee0f715d3bf9a6d31682504934341b355844b2340cfbd29192fcc1d2613a8a1" },
                { "sr", "8f159935ee0061a9ab522212db72cf49a0e5205b1e4e84264f066276feb650b704f9628203c2f0ae8e747cd4ad2512ddd6082a7add27d3490308b65073319487" },
                { "sv-SE", "72e248797ed9c69f77c0b882bab2a7b35f65ce5a84b7e87390a71d05a960f4b01696e2125c02992c4151295a3089f3fd212be4e96bafe5411d563193b6e95f11" },
                { "szl", "e26a729b1f433a121d6ac3c6ee39857804257bc565dad65de940a194a54bc826a9c0effe3befe58d1eed7e8bc9beb9e9c8561115ea45ec8b397bb8870922bbc9" },
                { "ta", "97a7a8ae7dd3870e550b242a70a5466a98295cea8c4bee4f8d6ccb37140ff1faa7ca1517047ad79e75632619782f9f5776ecc663d19d406be02e88593d8ae3ef" },
                { "te", "8af06a5145c10774bb2c6d9924d6c262010f2d19572694b8730bdc24f3ed865dcfeccb9b59f9951fc5b2f6e0a8a5ff2bd3d03e830b709d478fb646b60166030c" },
                { "tg", "2600250502ba6b831ab254cd17e6f97c0bf65261a28511e520db7f6ac12fc6fc42f52f3d2062d2c4491fae615db265680e834c0e49099e729d7da6db1a39ee54" },
                { "th", "2d734fe23f1e0ce27feb39bab5f94f9d0fbf538faa893a02ea27ea3e471d4969f474312cc0c1e83382f491e353663ccab154da6b1b3eb80c7a9d8c25759df0ff" },
                { "tl", "99e6822d501937e6001cecf5ff1812b172fd1641d016823af272da1393cb432c93f99004876e604b77172430b3bec5dee6ba4584339c652c6a561a567bfbce36" },
                { "tr", "6872177e445df57e3c801fdb497d578771251e7828e68b973d8a45fd03e7e470ac69c6cfc6faffb50e83ad67a239fff90cd42c1546c35fa5c929ca466cd22e10" },
                { "trs", "6512cc4e6850a7e086716d6252872670e51d15680cdf0022c1f6fb2696356e7b733c3d5daa28b86dba6d908135c81b09f8f8d74442279eb2751a46aa081a6548" },
                { "uk", "178df1f8b6f034cc53666b97d75ea38854ea769831a53acdbb482795b389788d58cd990874c9be24c4565bdf1dd9caa44f7ce67828b378c0b4bd109343ea57a9" },
                { "ur", "847c947368dcefb03cb978e4dea08c6101915ebd4bacc125c289cfa05b6412aa73d59480a3830f013770551af9346256bb95f2e8fb5ac5141ec36e9aeff3c7b2" },
                { "uz", "5fa2c1337dff2e2ab6f988721ba4a05886f3aaf11b6692544b917f062853901996fc9319c1eb50cea358c6ce85a3c6a0b68e315c96327ab564199a6c412096bc" },
                { "vi", "57b307c2bb158ea1af4ada163ee862f01882833c48f01dbb92008f5d189cc55dd1c83c0babc8ca212ee0a6d00183ee2b013d08e7e241b4adfa0dc758e91cfea6" },
                { "xh", "a54069ebc3b0e8040c27b7f7cb182f78b1ba7d78f39674bba4740e264cc12420549049e9ffc9ecbf14792681bd7e33191dd4c95a626906f930d0835ebb84d483" },
                { "zh-CN", "61d1f00eba01430a327fad4c9b7fd2e2629e75e929a760aaa7563be0a6de5ee4e561a80a4cfae5ad012891c6e055b43efd6fb99eb84082c1873f46f1e92994f8" },
                { "zh-TW", "9bef88d69c4c9ef06242a318fc6be7dbb808435e5885b033afc741b2e8ac506c7ad1061df29254370bd5387451893029b242b9a14d51350fff9defa575a28fab" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/136.0b9/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "bfa6dea19d8a35f968ba78642206a5cfa642bdf0acb2cc5521bfd06649998287a0a4a62b531410c0f46213e9c7b7c72c6fdfb48d201f33758096876bba9708f2" },
                { "af", "56186df8d528110845306898e75d9ac59e354f15621694e733b500ffe9e4426475e6cc7788bd38647ba04c78f7811288e5eaf01c7add4e922bec9915b1736c0f" },
                { "an", "d6f6e00207673c29f9bcb9165b36423fa8c98c6012cc7990000ebe0e677ad47de440540e0cac4505af5d8cb569cabc2aa6ff17410d63a10d8a6c8fb308a26306" },
                { "ar", "8fe85b5c6491069b5d61e71dc726a7934aacbb8fde9f273a02ac9818f0994529720784ca17894b4e1faf879d2255c6d70b48260afd2759f61587f2aaf57c726a" },
                { "ast", "022d53c5f1b681d03c340053fe1233af8024bf5554c9058f65508382e7ea3adfaf5ebd7e660f9c71f91edfff6e66e1422471a45d2c049be0785dd06718b05701" },
                { "az", "d8512da97bb039b0fa25be905bd4df1437576249a6d027076954f7ffbe491c4d68d511857a46ddc2b9e8e3d5165227057049b66b413819da8d6769b33870c1d7" },
                { "be", "b451f6fc170daf15bda502c6c9b9497f65a0cf2cdd9d8db4dd99f6ba791ff00d1b8140b9ebbea72bdc27ba9cb4358888fc0a4f1829ab5131bee5f5383e4885ce" },
                { "bg", "657dd9297a8378561b4c9216e63b34b846b69f6b48afc078ab3088c13d5f298ace56afb480cca0d8be9d3d8f30778718d97020c563e503b95786c235001117e3" },
                { "bn", "fcb19db34ab547681fc845dbd47e7997a7d1271cb6f155ec9d037be2fa9adcf43efeed94264939c8a545d4bae2b6a457f92a993c81bf4358d4868ee5a5314221" },
                { "br", "95fe1443e1fadb6fed951804009edf3df96facad1c22b27fc275ce8995346455d84c54ffba9d361ae3ff729b32ca6f7db5da6fae107424e54887b9d2c47a08ab" },
                { "bs", "fc395c499a41c184d484315fd5e9ad9f4d2022018b31ab154969eaa976791ff81671aa2a2541cbc21400efd1bc25115a82b43986de27e78423a9758ba44e1e0c" },
                { "ca", "c87e88d69b4308dc8a8d4077997075c7b031acc5945b6b062aa4adb8a6ac6f22c7acb2136a882208bf92643bcc811bfaca9184cb4ed0cbc75b0273a292c618db" },
                { "cak", "fd9643c22439f08b5146bd00332f1619e75762f610c43eea590fdedb45220990b6fbe47ac7910e247f47161723cee4ae3d05a8bd78b155ebd5c3958a050acf37" },
                { "cs", "422a65e64d5d4a30b2e1dfe013f74788aaf3d326f24c0723a9910bc2ba6f2cd5623e5532d45cdbc4cb53861318c1717cce34214d5bf73d376be16462d0417595" },
                { "cy", "19a6c85f25dfcf6ec0c7e3c67270068183d1a225ed3554244d1b65be3826037b4a03d3c848e9ad8f97444ba1ebea572a8ba8ddbe65a16f4b8050c8011e16bd5c" },
                { "da", "7dd988b28045aad2a5b85b563c4378c58eb7009d68a315f4b74a2259af387036f086f9f8148a81c9d1146dc76bd11b4cfe406e992bb734de8c8e98e14ae32bef" },
                { "de", "741909fda97c6d398233a97171a8fa066854cf6a9e3774df9563cff9a23e3765b296ef3be420c3b45fe2d01e21bec794e7d1f74e3e6f179ef5032cd41ba612b8" },
                { "dsb", "60af3093ad0e0c8f6cacf24657bcbec3c1cb1972ee9ef110633c3d56795871d17cba77a24f4cc56c9282406fd37cbae38ecb80a5aa2ee030fe01d9f16f97ff4b" },
                { "el", "d8c6964f248c3a261d85afdef96e96230108b71b47611b174e39314f4edaf30bef03f747734eb651186c09619b05b3f5f5ca5e6d34e541748ea96fad4370651a" },
                { "en-CA", "f0223d237ee7a8bb9c35297f7220c1fca7397f9204da8a4863ee67d68b7cc57362030223c0a84c54ca391602f69a61df8088b2d454d1141d33ab2f606a3d962b" },
                { "en-GB", "ccab5818d6bada301df4ee9f76ea5df9ce08695533bac83a83d23f537968af464d5a941855464dcf4170f5dedc345b72ebd5b483fbcd6996d2ba90e8f6eeccb7" },
                { "en-US", "0b20a1d1e15e89927bbb3612737c3b8bcdf7f6684e5b5630b81c29cd08c3968a3e457fd0563ff7b86ba71f2f124fd8f005bf3809b8970604e890b0dedfe46225" },
                { "eo", "e18af31b6f379a16dd29a2cacd860d6408c12fbc3d197c94d48a739a1279d7bd2638fede4f5c09b14111896e3e09e833cc475fde3cd73d7555f938fd082694a6" },
                { "es-AR", "851b2c1db76822fdf5d0841391c008d94289e44cabe73226506b0bf80f92fcb6c4dad7ad6d9a5ac52c83edf76f0ffb08740f4b5c43c3e852afeb45304b529732" },
                { "es-CL", "b42f590b084a4db8f48ee0ca2854f0cb73c922a14c6c1e4b4242a728117a97b12ecc98914fe90ba0e43fa4e213097b5df559a7a8dabe7f193fd91e41bdd900dc" },
                { "es-ES", "40b28cc04f5fd2485512a9825ab99c43e7bbe0174f8555941cb7329a580a8e18f4d3bd1967a3a61a4e55139287c5bffb42a76db05299e815736d907fd16d8575" },
                { "es-MX", "e26accf3f320dbe786e0f583553d3068fb72aa4cf4aec8e0f287f3763eb2c2ae430256c0d560bfc7eb92645b4006066e699481d4ce7c0bcf7aba5e357bfd1240" },
                { "et", "c48b29837f0f897eff1b5160209529060575ebff50ea2ace5111c477e46619d437f1f9f6a949bd10e53499a566ead4375d6174040854b8270b4aa70f1d41f4e0" },
                { "eu", "97df6d964f9ae0e4097239e457d5d33062be7ac7d90c12a27e82f2b2add46cf0841beefedca0b7c64925dcc346c1cf4e7afd9a6dd2481c020ecb8dfa32dcc834" },
                { "fa", "2d6a4a51e822c87646165a3fc26ebf76ec3e8d10bb703a269e2404df5ec34ad9b6dd2599838091e54081b12bba0b856dbc35236ac6967017f9e864f2618398d3" },
                { "ff", "8459a4fd28480f8f4fe7b41078b2b9b6d38fa6d25872593056f2e2532899481333a4f980d9bad8b34ef7849248d1c8af715cf170b1b0446c59667ef5b4a5315b" },
                { "fi", "0c6650ae185d720052d9c806be2a838797770c72273a5e40a32c27176e58692f5d700e28089a11fcb7a5feea3acec61bf0941ea7b87ab1e54ee3cbf6202ec4b2" },
                { "fr", "b3821508512faea5ceef3d65a6632acc915f44d346bc6069d20366061ae9cabee9482d8571d2776a77244cf0941eea4b992be80e797d359a709d1d0487d66a51" },
                { "fur", "22c00289950a6e9264fcbd6acacc82ce3f72cdfb061e095d9ebea5d77df2f1bf8aaf7c1d9ca131b592b862a9322995300e724be0384534ea144bca2208c9fa0c" },
                { "fy-NL", "43e69da7a0d70675acc6d9235a02299e58665e3c49719280f669ac54a6ba13bd8debe0354c4920ee5683a0784487d7dd6fae05267ade3a948540438230dc0e46" },
                { "ga-IE", "7f920a69ee22b5989ee435f5c18e68aea116e7fd3fb200c7024ae98faba73faaef4e4f1f71043fbef2865c86282a53fd06967c3f063299de184a6cab10531971" },
                { "gd", "41ed48b788c391974ffbfba01571dde36001bdd94410b7c54d781fff7b58ad9d70f20ac79b47708e1174d67b407befb641c86a2194de12d53e8814a48f296d13" },
                { "gl", "8ed98c66226070aa80859c65b60f4f1add2208e1cb7c20dc3ffd113c62641587ce0a7df1eb0d984364da93fd19cb65b4d4ca766d321fb67ed9e1d2b88d22ddf7" },
                { "gn", "0a6e28c62dd97d2e60a681708aff736ddbc3c881af9888feaefc79fc8cf80cca253cf81c1567bae7c772abe30c8d8240caa11edf6f40928a7e9b364607249b00" },
                { "gu-IN", "9b0f77d989ac60ae83234bc615ab61a521f4194b3ba5c71683d2d8b4424a3f775f46decb1b8cc1f45c73095f2590b9fec223ea4df5a9ee495945e5b654616fee" },
                { "he", "4aca65fc44e3f9a4dd8c59dcf5a04723d8ee7a8ffc322955b0d6ccf62b9cae0bc58cf7f8b0201ecd057241a17d1a7d807b07d8df1f540be7cde8f86ae78bd69e" },
                { "hi-IN", "945325ac70ef2528da8b5c8f8831f1c50c7fd53f1685c709639a46697451bf8e0e8630f6212ced4832ed81b9bbe3cd0cc5da556f019d3e39fd0494d4fd045b63" },
                { "hr", "c20e203ff227433640b2b7baa7da50cf7c2fac0c4f6ee4231daa1c129a579e0254e4ba3ed081b4519bdb00b5479c6afe0433cfaf9522b934f93457a3bd660807" },
                { "hsb", "886571795da179c37785748a540b514b1d5ff74057b256c4af47f9e46be8a5b57d6f78227d9ac209ba80c08a4e3683f8e4f2eb1ea350c645af7a26b0ab67a4d1" },
                { "hu", "13cfa83d8f7e4f26de110a9600674e3300c366c092b39d5070bdcfdbdf55c5245461da8a33b34ba64ee7c21c10962b0e4cab781790ec317ca497233bdc7f5a27" },
                { "hy-AM", "3ea4453e1d8927d4b26f75152fd8ef7e3bfd0df5e3cf895ebb6d9a20c6440dedd878558f4de19e47d286e862bfea32eb4b072a76f2a0a27400c263b4ecf1a3fd" },
                { "ia", "935c7c5f00d8e1eb34918e3f2f33fcd4efb9e6fee0eec5710af8c5578b835c96a607cff82253cdb4714be3500279194608029bf788179b3c2c50a999a4b2cacc" },
                { "id", "114e267b9adfbc68e3b18d30c40536af714e0a6febb8e5f83fb742a23c7e2f452c15d31aaf3f538a4e73a24b435c37dc55a7f5d7f07e2230765fca444b4f55da" },
                { "is", "7e89816de4a77b894f1ffc3ac3af2f8488fd4b8b314005a336d15884a85d0de263183fdb26e7120aef1d70a45d97aaab965e1f68882e5e10d83428b842d0a42d" },
                { "it", "15e3839e0ffbbe12be2814e90d58644b7c62a9d4610d5927dbc665a53821993ad1b6b3a99a3e40b25ea78566ab13d2efd149002b62a79c10ae837401daed7326" },
                { "ja", "665c8cc09e60859bb856a51e8d92ec97f19a12e10550e063ac29b55a6b1b88e470f82aa9d2cc81de51f6f849b80f660f476ad6f13a554909a0c6a10d9adf5de5" },
                { "ka", "eefe24a18fafde8d431d33b43536bf7d3ca066c09dabed508bb678f13ebefcab46dc273725abf87864cd8c68251758de517608e4aa8a39d8b76cc096d5e8e402" },
                { "kab", "9b9d9287dda90cd6049be1cae30c1bdd576a5ca5a66faa6c95d67c8cba231ffcd22d94c8696fcd4b096f7470d11149142844cdcbc2bedf571d7580b867eb216b" },
                { "kk", "39aec344dedc1d68663932f1597901f2cf389399de449b66d3c0bbc4ab8872a666a619cdc18fa1807366c81ebcddf57f03988ee0ddd5db6393470fe82876ed20" },
                { "km", "c38c7b39789593d94697579011b9a6338968d4b99baf3488885b31e7c481f0bad3c6b756c3a2f4766b75552b9f285176b79397b891d35dcacd8fd020706d0a14" },
                { "kn", "6d95f5ad5e6ee79f2af6352b1fb185e022614d2ba671c65aaec14b052596fa432f07b50a9d0c42a04ebb09c8fc1daabdf223214a5d5b12f70b7be8879feaf04b" },
                { "ko", "52a8d956ab41b9625003f5b4eefd4d232e86d40e7107b121d172371beba07eca7d868d7cb043135dc44ff3bca13a176052877bb73577f914b42b338202974f83" },
                { "lij", "7c2fb6a26cd1cf9087ab5f764344769342470f968293db21d6d6a7fd08887e6cc0c805edafd358be5958d70848f173b92381548117510128abff1e9e332b42ec" },
                { "lt", "9ce037a8bf51f2c6b8e65e54e594aa1fa29f13105d19303e2a48c898bd57185516a88fc29b9799350059c6715e7e4a3821c3624c3e5b8a981fe7dc775d6fdf8c" },
                { "lv", "1891d1f6d0113683c74a9a209eee8830b00c1159d2f762560ccbb415c99ba5918dbecc7bf79c31999895b992577d69b5ba421678f759051007e5256ad4e61b88" },
                { "mk", "0a8270ec36a9fcf1fa4e9490141954650e508fef209320f349e7cfe9f0685e6fc26c538f1e67df73b28e50ac92feb89a16a525d9f28f7f7094e593aca9948cb7" },
                { "mr", "184d75c63390b584302d8bedd84c428c5b3d43ca8d5de96ee5e004a4a2297c3ef2cdcad586cc19e55e138bb3c2cc23bebc9c168b3de649ffc590f231bd7401cf" },
                { "ms", "cd868b1953bbe1126d6a59db97c1a5e398a78a95c2ab1eb0d5f1b864825a63f66193ffb9550a9137ae90721c12c69c2b6cc0f3c9207a817f84dc77c81aea863f" },
                { "my", "a98f0e43f33904c5fbdf15cba970d832f9c8ec193aaf7522ba90fdfec960560c3d5f006cf7dd194586b06cd7df285388f6d2d9dd434c893ff899e0063ec926ae" },
                { "nb-NO", "afac1056c01f08de54a819734b74d9e3cd84ea632c2d6ea25f1ea6fc30fb11adb3339e975ce0250e9d9840ffb29fab1ac13bd87282e229198c91c4b58f2f1d27" },
                { "ne-NP", "2d9ed733b8cb5475ab72f0a3d4898bd551a92ae48154254cb75169ba707045a58a7f0f250418796e352963d4c2043b8bb1777ce854d96ddd75f2af398b33c4ad" },
                { "nl", "a038a5e045df3b02e20749113081a5e393dfda6dcfa254c21f3abfb2e7d1e71b803d0016aafce2591a19ffec7e186422830eaefb8a4bafde2e479e7d95c782c1" },
                { "nn-NO", "dd4aa5ee6a6756464847c713f43f39a17915428c890d226aa83a4f5e615bd1f0f2201b76a764260d540ffbc0e5997603b743cf00a037287ff7065050de0dfdf3" },
                { "oc", "6684e59a6aa7e21be5882d3311633ea5efbb66487958f48b68aade614c132550c5ca8f4e93c05c60bf500353ac4dcc4ad3a43e8d80d5ac5c270de1afe2183c48" },
                { "pa-IN", "8e9830bed35227a8e21260301ed413aa2b3b4e4696f7d158ac8a21f130899a70b98248de124ba06ccec9044e679724b9d7d061e494e5082e8d933ba8027f7d6f" },
                { "pl", "d948b4eb0f30b71d535385240ed0811f703e28659d4302a54ccf33ea1771063b521f13c748d7b851a0ea7a8e1c7829b0c3e466418ff156dad3902ba0470a1dc2" },
                { "pt-BR", "a467610afea5be168d5b5b6b562db33383d1537450aaf5500d8469d43c29f33e837877320782207534a4841df21df97136a200ba3f5620ecfda4e0dfcef6a66f" },
                { "pt-PT", "a9ba787fbe7ebdf0fc276c20d3582eac1bdef4f047048db390232647cf75a6621678d08bdc8004935b5cd980f4c58fa46a44df9b3b4621e35a61de6af182a9c0" },
                { "rm", "f5a89e185a73d2b51b9ffeec2fe886cfbe31cb9b2de5be6733663bc5c8f9d182571e5dc9d595c7a0ccfdbd86b299a626118ffa9fda5ce23fd6fe99db2472fb0e" },
                { "ro", "6145b9547dce1a76fbcf8d11ecfbbe90c857f7e49f5bda524b9a770582e1581a8e9cbf2c18ae0db655400d339233407723f4aac69767fd17d9d7b4a081834b25" },
                { "ru", "50a7cda0e0437304ea1f7fc4493b7a178394ee67a29faf2f4ed743c85141f3a20614b41cf6a736e7dbe14ff5499285cadef78efb921bd478ea41d49b54c29975" },
                { "sat", "a0d1fcff3a79c0e2a9a0df7296820805d5e157391ed8e528422a724279188eb27ca0b55fc2d4560eb55fec9d6e26341eaad46baaa0a3d338dae7bd1ae1fc45eb" },
                { "sc", "014aded202324dc3a56b558f79dc8586ee001844170f3f44d0587c4c0d262a464982f00757c03053b926c6c533a16a5debe46251dc9416ce28dc0611765257f9" },
                { "sco", "ea8dec1832f6c1b21fcdf96991e3c255bec227667555d7bd0e5f057c9141865275dbaf2888c9b52d2467a190e6c91804ceec15b194989f925f137c5d22cd40c0" },
                { "si", "72724ac4063b7919ac38ef9af626127d11ea6393580ead9343cad1cbada515c6b0a56f1e8b551e4080cc8e881a759f055bfee62d2a88fce9e4e47fd9cebef71c" },
                { "sk", "2cac8c3e239974104bbdb18a6b627a855a01067f99af230778a352d3932a69927aa3daae2a509c8b550546656ae7ecd30b4fbb245cc331420fc12dcf343fb034" },
                { "skr", "b0ac1b2e575c4e522313cb30edf9465a2eb77730cb125cf17f4b987e6322221e20494e275602280481fc1544a9073808fc9585a9ab25b21b7d632cb066ae9842" },
                { "sl", "a7610a6a75f281283a6eb69adb0b56977c86df2ab82e378212c43e29173ae8fb39d3f18f7916dc825f6a392253b6d250809a3f77b1452bbd75991cfd33823922" },
                { "son", "9d63b1639a7d626e280388d1f347608869d8fa75ffa6ca38b954e9bf5ae562b504cfb5aa5b6e8f80682f9bfbcda725306ea7d9fb786f9b9f7e695b7af20c3929" },
                { "sq", "9f3206c025cba9cd8eb234717cfc85aac35dbdc8b2aac89fab8a88ec61b287a6a6a3de1a8a18f18f9cc737e1752125075528b2eb55b51623507245889ae4703e" },
                { "sr", "c539b663040f7c4340ae99ee814c78fad93cd795b809ec3fda8592bdc9777bd7f6c7f52b0b61cfc3d1beb9669a745cd7e9b2489a500e4f508479b37451e61d0e" },
                { "sv-SE", "50efba7a2add8f647499d1b780063620436b95ccff590b6e5da8811a575bf66111a83e3b6ed5220d8d0cf99b4337c7e526bfef09bbe61255b0f948dc88985333" },
                { "szl", "8d0cf6386df4b2423144b98a4c6a2bfcf280cd026b948dadb8d70148680702741a1dcd333fc22406f0cc74053d64bc8b5159c0de622e604ef32ea30b323706b1" },
                { "ta", "a2d0447bb5124a06bb3ea6782ba52b1362261d17b4047fbb11d86def8ebd514f59a82b8284042e1dce0b83239f130cf3a5d927b6eef1b804d91eddf40e16c99f" },
                { "te", "da085135c8cf9f4dd875157e56efe7faa85ef33ca0809efaf1e89d48d77c9d3e61ffc357ef0411e72026915de5ab827901ac0054e012669e01050d379e5d01bf" },
                { "tg", "3f4a420cb8b0b2aa1a92620e550ae07cb6b5249d58918c5906ef3c7158a0add8f08624c7cee468e889a847548b8314b51f9ec976b6cf1cbd29770be58f98adfa" },
                { "th", "5116961a50e14e66e0e799bfe28ed72af9c9aad7e526e4bfab750cf9cddd897204d548258b3c28dc43657a4b5f16c255b915f9086038850c654972de87720f38" },
                { "tl", "ca297c4cb4c843f50d278c338120fb00871dfdfedf8b1c5114556390bdd380dd0289e2ec1ac28382b3d7db5ca790f350f43983c9ed2242b48b4f4171c45f7c5a" },
                { "tr", "b7b4908bd4439c56e32346fd0cc4331b555b04dbf57f761f1d3ff0498f71fd7be1482361253a893e3165ff83097c8247278b1172207391184af3002c668088b6" },
                { "trs", "5419ff23b3c0b0717cc493062d362ee274959f57dee00b1d01e63f01b2be045307c8b73ab1e2681e7664284bf02ada1e12fde5d4339ec6007c2f17d1453bc4b2" },
                { "uk", "a5e8310b6351faf6338f328f59da052114650227c09f9a2c24f260dda3f5fceafa315d1faab2698fc78f99d780e9fe64297bbf7edd5f882393e9f41175692694" },
                { "ur", "1e37f28d0f93735216fe8f1a43b09fe35499a02027be3279d1473428b71ead9407f299cb81e4d4fed20c9d2c9c60d0de3c1eafd8c26979c54123adcaf8ab3b54" },
                { "uz", "a87967e0bb615026eaddd204cbe9cc1a2f4aa31940f026b7f9985f9b3c073e6e382eca1c36af3e826c36eae67bf9c82b1a162551ec36cf44894eadbec3356791" },
                { "vi", "fae6d999427a0308dc3d203368fe002839b237a262fee88bdb4e9b17587a4346fa950c05e312751b3af36109e9233d121669aedeec7c78e8a851850528323709" },
                { "xh", "f3db54ec867fb4bf82231bf34cbbe75abd665cea2135e0049e1ca205f6c44a5d8fd2e2e5ecf4865dfeaa110c4ad1ab8f14bf251f6d5a4a2e13b66a8b4e6554b2" },
                { "zh-CN", "b5ac2998d7bd0a5cac3a1a9a06795cca865287eba3473481e0bc7398555a5ec16cca24964f9613790f83960fb440467e910a2231332bed97604868f4e82b77a0" },
                { "zh-TW", "87e46ee02e34807c6a9bdd9ef5619bc8ba753ea2acabc6439ad7ff252274275b02c7fe8c23ff15d72882385194557b7c1d3e6da0bb45e0e8f6fb8eabfd5b4a76" }
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
