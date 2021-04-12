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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Manages updates for Thunderbird.
    /// </summary>
    public class Thunderbird : AbstractSoftware
    {
        /// <summary>
        /// NLog.Logger for Thunderbird class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Thunderbird).FullName);

        
        /// <summary>
        /// publisher of the signed binaries
        /// </summary>
        private const string publisherX509 = "E=\"release+certificates@mozilla.com\", CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2021, 5, 12, 12, 0, 0, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Thunderbird software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Thunderbird(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/78.9.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "8a276d5cf85aec6f42f27a35a10a26b3c422687737e8d55f4dda3bd42d085713759beb50a809881d7fdd3af9d621f7ebfefdd6fbf8e188f97d48f54803349c36" },
                { "ar", "722176fe000044aa10377cd4997328c779d2ddb0d776a1da85655ef2ecdb14abed8bd371b3d9d81b451c34729f5c3ee23c6af2beb57ad6d93c38bcb242b3592c" },
                { "ast", "e146f05427bbb002cfd82d0f66dc5e945c982707530e6879a904698b434d4566f1e0f375b5e05a107659f0f21fefb73e44a9db488c5ebff8b3262ae5b6d7c503" },
                { "be", "dcc31ab981a1a725567b46185dfe620206eff51801426c3ed00dd1cb2459a52aa30b6eb8c5453dc11d5f755b0d92e44325bf41a95d7320282e10b0cd0d349a84" },
                { "bg", "a06715fcefc567a9414344fb7d47b1759301fff96eec952ba932f20503d665b75a39d55bfdae88ccfe413b8aced1e43da7579dda229a944c7e8bcc6149c3eb88" },
                { "br", "4f24527c7347698e445c03767003edc50c2805bd6da3704d73584a666a76becf27aeab5ed93cf828a64034f1b94a2be6b6b72100a191fdf663d27c4d8329d214" },
                { "ca", "357cfbf1a4d08615267e444c73590afc36b9db9e87a9d5316c1a3f5db20da122938b2ccc66827ce53e549d0cc48ac48b01309d39248e6903c3b2a0c8f0749b73" },
                { "cak", "4d8a182491abd97e2e13c81df6de8e7bf65509fcf4e127b6b8e449ca831ec17d28078fad25c032dd7a6221dcede8b90cecf6a507e13cbac5aa56164d12ca3cba" },
                { "cs", "d952256df069f04910823857bc0d6f193096a9d222835f4e865bd2f4852912945fed1d3c924cfdf6ff7e33844a5c7489fb598925bbcc95fbe981729c26bc81c3" },
                { "cy", "cad393a2561692fc844f92a5cdffdbdd15ab909af8d7acec7328fbf15e6c632d7ce316d3d5a80457ecc2b6ffd8f1224ffb136038884db367498482e8b6c7c626" },
                { "da", "80dfc90495a1f1d8b9d7267898db1dea39f3c5e01f5b5033e0ed26161f90ed69f6374bfeae7f9525a1d32ef70c9b4c8b38f6b555e9fdb4dd12a09e037f0b0e87" },
                { "de", "49682913b45edd4e938408a432d3c35b2f1bec250c7af221577b07cd7e4a7c765792f78af830d3396e5885c241927fa8265667b8f6d67ea3acf5434ec6579caa" },
                { "dsb", "67280f1a393d777cfe5ebc100a5faf438b6b13da33f2fdccfab192abf0ebfb92bd03268e86e1df040822df0b62f0376da3ca8558b95fb4837eb908b19bcbd790" },
                { "el", "bcff0a0298bde18f613595a9e988c441779b72ed36846d5c6ea9e9723f8344bbd2c28c4c50d8d6ccef896ba77bbd99fce3eccf58cedeba4178c480067ac30ced" },
                { "en-CA", "bdb820230630b6612dd56939866866dec4e68d6595b0a8715a7767475b45e0e45dd19460039c94410814581b1a2428ba907b586ce44e393490b74adddfa99b6e" },
                { "en-GB", "b6b932c1454f03fb12801540e1552be4dcd51157fdbcffa0a94288d207827a3babbfaa0a5cb1d1d47d087c1358203035e8cdfd4522dfcf84e1b6e8f7f628eaa0" },
                { "en-US", "113fdf9be245f9abb0bb8665ad5f6d442853c1169899742091dce9143c950a58e06d8e88a3ccf6ec46da0b285069792a718701ff0bbe15f11bc4b893a796ab14" },
                { "es-AR", "a1d13a8d33111a15689c966ecd3d1649f38b7633c0f1855fbeed7ea50dc6baa91896879b483f33bc2e1a0c05ec4903d88a650e34a3cbd41096e6661416a234fb" },
                { "es-ES", "082d04d7d29ce6c1df137540f6585e7373b8bff59494b4097d308d047e2a3ed1177e1f14906d8b1db771e90b393d345ab53a3a3a3ed1c94ff5bbf816088e0c75" },
                { "et", "efe87ec245c046643f71476d27ba23b1ae14aaa0913bef65b358699626b8cecaef12b9e8ae69d90be036322da98da87db9d7234edbdcda67d1c655ba69059573" },
                { "eu", "47ef4066efce465431c58792a33a367a607f770a93f777ce56cd20034e1668a43830b5b0841b5b344641a7f5cc930660122c79da78dc694cca4b3d6547e0707b" },
                { "fa", "27d4906e219fc80d64ee4fd8c95a8b9d876f59ab30de849e4d5499b69f3cda4b2eb29b6efaf5d47421e481211bf8ca5ade809eb49ee3e411244a4106dd262371" },
                { "fi", "9d73c7c329b7a974cec4df6669880f8707719c5c01f075a8c10a4c9215ca4a39a302ff3d4cb84d898c0467b33254daedc482565f5331a5477d965874ad3ed232" },
                { "fr", "21b6eb597eb4dc24459abd08d7a7d73dbcbc585186fe9502163172f4e460e59da76d620a5a15889ee61d5d084b46b9f52532fbe13c7f814224d4c53ee322625b" },
                { "fy-NL", "3a76eebe86f13150cb5d5b9c0b0e6313cef10a043540c2204cf403cd8e2f48b27248dea51628686030cd84b9470699e21bc12a3df770da8bc43f33e5835b9721" },
                { "ga-IE", "07fe0cfe98902fe5457b27a650417d8e579dd516b84e0b1b60c768180096e820e0f5dc9d89d7ac2c076b7d08f4c688812ef1c915da113c5895b98514fea74bad" },
                { "gd", "4488205d829ab35002503e87e797ca11be538fd3555214012758196a29e012f93a648c9f940e00d81a9a44b0208bfaa503b366c2ac1fc83c8344898420a8e46c" },
                { "gl", "39133483b820c1beb87a1440a958f8bf8f1c990a777952e9262b3371d2e65683788c1074f10e39759a61fe470cf5ece654ab4d52e1126a20dded0473487c1fad" },
                { "he", "1ccc88cc1a8e70dcb6e3d4add4c6da3f18e1e0a77c91c716bd8ab3bc435d028930be6911ec46e08b0561e25fdc7d8f94980f3bd90a7e8783fbe33ccea67de135" },
                { "hr", "b6da602eb603bf3c318fd680443901946be20dafb0e4b9e5e1799e89cfa78b592bb7b8a79c6a28869ebe0159eeff0be880440175267a91745ed54d8fbebbae9f" },
                { "hsb", "2feb739c06e0eadb9dcdcaa8717948fe9e6be170982ac425e39ed7ef4648e142f8e94af9445cd73a1cb051128d8de30476196f7ae941037de5aa5ab2b394e0a0" },
                { "hu", "9e698f20b7f0afcf137668372f01e1f5061d9342559d9ad162f9de37cfb180872d1bc1e180f432f83929db3b221f34f27eeb07b97ba7fafe6533c59e53f69013" },
                { "hy-AM", "8008251a755a53a3ce570e4af2f216b79b63f5d21426935d647b2aed1082082cbaef373e02b5ee3ac1daa0f643a95d13968cc33c48e9ea081b8440e48527e2b4" },
                { "id", "f3bcdd966cf3aaf485586e39207ecd303b05a6e23cad86c32ce385eba161f5672ce019f50355ef8405dc9675148935bad3bf7059f5ecb78da1a4a2cf0b7a50c3" },
                { "is", "124ce86742c211fad0095d73d5b07d9ead9d65ce885d2769828d03313d367b92a139f850ff75fc46b705f90d64d1911bd2eaf729c818f542d4456dda67e86d3f" },
                { "it", "5d61115435bdd653b3083232859c8aa1cf630e2a43250960f91308c1c4294ad04798f5b180a31cc3e47b02f5195307437a512a2daeac5f4e7c6f3ee71da9496f" },
                { "ja", "d19f5907f57a86580976e0593f15d1eb0e17211a2397908c7ba42510e0935c7d76c7d40d8d99c4e3a26a758ff6f80a7c6a2f413e6a9f10b30a91c047f1660d4b" },
                { "ka", "234512d4c1c5e628480e891b1593b1cf15101a4e4f12751bb5ecd27b677fb6d4a2d9d6a6773704023d612737a746a8575bc4924a5737549fe19bbb6405cc494d" },
                { "kab", "9414edfd5db967dca0574e03adc792f067b1c4f5a88d6e4d77211f39bf8ddfe9fca5bac8c1b727eac8a7009e73732034735a6ec960ce61dce201cecdaf9def02" },
                { "kk", "5a1a75463fec79458b9fa1165b2bd7a0270c6147422cffb865df742b35c678697656e5cb9787e14410f81d1878327ce80a800bfd8cf75696d8bb419562ee079b" },
                { "ko", "613804f91747f15afa379eaef43e41674ce44e0c6255af2a567580689c3d4b52a3f633e28682fc0bd65320976e11cd8f8dd23bd6c513de49d829a18fb8f25e49" },
                { "lt", "bb2243b0eac72a610bd7df42c8a93efcf8558cabc8c2c1c08fc1f951c00a5ceca60a85df3f279d04d94c823701b1d21dd2c3d94c6fc23187b9bbd0bbedb37cae" },
                { "ms", "1e446c59003f898bfd28cff547a42fa7103b0cb396890dcf849d5887616700ffdec8c0a051e631d362a78c281f98ed52cfbc9d92dde1c5cb582a9621c5432784" },
                { "nb-NO", "ccb7458875263cc92a8cdc6a029214de481767b11e367bece86e95560d14f51775b8661e45a05d04b6ca818724038aecee7e3c4f22a48acc9c3be369a6d715bd" },
                { "nl", "7a6f444c0ce45008de44a21a74bc3fef3e7f413daa3b35da6aa7e04fe7bb98918a1f63d6369a317d0581b336c8f03c6169b5c4dc281bdae1585bb49d80dcbf62" },
                { "nn-NO", "5cd3857a0d2e13940c21aeb0e4425c24f515921a2675d7694c3bdf566be100a08146a8c55538a46f4408f8aa1fc851d25207c4a1d7d226d77e6b12032f3a357c" },
                { "pa-IN", "3aea1ea887ed65bf2612dc79495f0b92c35282ffd2e140a628322c255029a03334b07b50ff68355018bd11a6abfb10985b38b99306a14a66354c2b8bed2b6667" },
                { "pl", "ff6d47ccd7aad6076545b66fc4607c7093315cdda5638efa084b7844dc191e15b9e2d331f21fe8a107b4577b0bf17243fffccf5142c96387c7f772f85cb4f879" },
                { "pt-BR", "b29a0e9e3df51fe411d82b90f5a19208049e1701ca6140eb30bdebde40824c5a739551417aa95c93dceb9fcf78a019f5af6ce1f549790af12a465aa1bd561b22" },
                { "pt-PT", "a0ca4b6e35351b6d540c8280c852acb26855ec85f3ee1cd2d273e3e80c20ab3806372ceca68f490e4dc4188d76005d6e05335ec2460c7558078f8bb5bce525fc" },
                { "rm", "381d8fd02400375feaec16b169c536c89dca5d138255dba1e3af4eb34cc3fb74b6f383c662126f0aa2caffac4e1deeed182be9f92180ce162a50b9305294699f" },
                { "ro", "2abf1f654915c68a1a888ce952126a5e4a5c3690a1b6455bdb3a138038102377f5c83015e366714fc0e8aae5c6834df09a8a994b9ca24c1b709cc3a0c5bd2155" },
                { "ru", "19ead0118893e09664b046b1165d3fa55b44516ffa487d449116e9155d39f35f5bb423ee92bad0aba51d0df070cf082c1b5a967e157eab502cb90e134c8781cc" },
                { "si", "5dbdbebdaec0f75665a6bfc6804a0c9a45b58bf28080415912d01b570bfaa67168149358566779ef5b972916d86c81915d25e06df87fd83d0845831f6cced0fd" },
                { "sk", "6a1d15ae335e4f8d268cad093c50da329314c9f758c2a3f701c7f156b1636beba11dbaa9ed997f15612e24d8df120edf2ea661e739e1e012ac90c98c014798d7" },
                { "sl", "022249b45a655929e38cc25fbcea213d73720cb57de272d09ca90c4bb6c6cbbf695a152c20f779b25523110c11bb8faf33b2135b6108293c708014578b17722e" },
                { "sq", "49558de1752395d90b346609458a3c1ad2324cfcbb4c82cd992d809e4367d77579d4e14ff8715bc6e31b2ddfe75b3f5f68a592685f3f02e58f2394089303bd54" },
                { "sr", "cd511d0de38fa15da8803709750c71948b0e1dad36303262e737ad380af09921916679d578a7933bd7902b713745e6e1aa6888135ce0cb5f226feebe44ef4f3f" },
                { "sv-SE", "55811339f22b346e5767ab5e36a0a0eaf270698e860482ad0a42511dc9df89d562a13142b73c76610cd66b058854d67d6782e044852413c0f4c1a0b2404bb2cf" },
                { "th", "77d19442b31f7621ac17f85ce62623dc5069c97eabf512120ea1a063489f1f900c1368e21de70e489c33031a6ce3062c829fe541510b32b824132b5a2a89513f" },
                { "tr", "17522d4d023d4c4da1f382851825ffc53d138712b4dbc403cae3666b348c3afc7156b330f4fd9911cac3450b036c1d7fe4ab4cb808397a27f3a440fd4ae438c6" },
                { "uk", "edb1113d369a991efdc39e38e3e7dccbe1ed73be9eacb6d8c82f2c634de2e3f880562dd3d84f27701df973c04daa16387278e6cbde7e12ee7a778158ba996260" },
                { "uz", "ff2aacc750709a76db1045c3b094165605198255686e16d8cc8719a4eb64f8549cf108a30d96aed3f59ebd8bf7cf9a47ec480a4456ab06a34dfdfadfcc341a0a" },
                { "vi", "8c6201c1d97efcd53f233a9ccf6b4861b8ff57127e5f9638bd17b296cacd164d69c3ee9303038c5deb4725f88d88d439f1e3402016fa153b6bcbadfb0d870b7a" },
                { "zh-CN", "92f9c62fbe5329c4495c008c8acea4b310c1916dbbed3c7e01a466956458f54142fe54a5a27194cb72bf77cdc68b614795282f5bb3e8875b53eefdbfe951dd26" },
                { "zh-TW", "ca5f009751b96e614a8fa67f0408b866a54349b1005ebae98a1625eb7e5a2a344c4e9391b5306322d967823f15706882a6ef850157856e5b91227cb7d126dedc" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/78.9.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "7af09fa7a8489db59b9ae8e75794dbc33292c4ad0cd21396ccff48f8377e9ed4effc724216698ca565e30faad3d253240c79e4fbf69f2b959aa663c444254415" },
                { "ar", "99768ca30e05889f3b9c82bff484536aaffae1e6f28df206bc78eb078580d5ccb41535ed5b4294e0ef7635596e092ff3266698b3eeafd19363ed66723d523eb6" },
                { "ast", "319c579f8116bffddbcb5033f6870f3c4bd2e923580487372189f369c67d382cf4df3815d20ad2da6a37fc4bd10144899bdb0e708b086192e287326f408ccaf7" },
                { "be", "1f27e65e8725a807b954427819a8f5e507622faf81bfedaec8b2cbbb8a77bc95d49c643245107c62d06146fb4d470694abfe9ed0c5589d7b5d2ee84ae9150e15" },
                { "bg", "de8a51557e35bcbe0e03fca00349d85ef009a01f8ff41f35a7dc035587272bd010cca5cb481c9848e40738148ea59d2f81f40491d678987be56bebeffa285dcd" },
                { "br", "29de665a9ffc8d47b0893a5f92b3479f0e8c600c193696d185ccfeac6afc463bdf0e3525b62d8ddf14477fabeab6d6ccbcaa41af0faad6b01ca42b3da1deb85a" },
                { "ca", "f2eecfd81c32800810f22fb51ca98e8cc0e13cd9b1825cfe998f061f5d96d7bb687ae2babaa25a252bb402dfccbf2b02741cf4a5e9ee472f071ee21adb43602b" },
                { "cak", "fb1ca56a3a299b623a314d687a985b45687c28ead5524ba04aa7392bdc9f1a39fabce1f22a5030529300cc926b56aeb5d74e78648d60885d7a563834e72ba69b" },
                { "cs", "4e0807e00769a3769764e5e12a43b0e6d20029b147a3040b1a4c0ffe0df303c43b1d80b47cd2cbe122f0fb340449fc9f2e7b8a9d5cbc5b9ed8457850b83fa6e8" },
                { "cy", "d6703087b53f2a49d71ed38bea3349564c7ae6b24dd6e1f3d759e90faa2473394896c354bf982b46e2135a750c10cc7f74c199be3aa2852bbfb8b4e6764313e1" },
                { "da", "87239d82a39d37efd33f872a24de7c7c528cc05e5a83f2007e25d942868bfe662ade8397fd8c79fd613c99f36d905e311c33df47560fa0500ba4e4f12f6d3faa" },
                { "de", "4f5aee689cc8d02039e844737272893e8eb95fc5c5c95cf9074f5926b3e81630d62d22b79c0b0d8e73657c56189f86be10deb35bea4205edcca123afd1fbbdcb" },
                { "dsb", "994dbadce8d72064ad8358993e818e82db252c7d67fe17dd0f4f4dcfb43f3725d52d0d73e4c26e9c555fd1419822667c11afbbff8da46a751c9befc89c12aa7f" },
                { "el", "639469e01805040281b6246ebbb91aa3af0dc1b89cd6d338ecce88aa944b1f8f1a713004f83c68112ba949a62a44e9cdb64882c519dea67eefe82f90cc5f807c" },
                { "en-CA", "a6bcff1e589e0545aae7ed50b377dfed67535f2701d0e999673a574244381a26d0abd893bcfbccfcad7229c101dda5c0fdb23aaf9ab908877c98150d62a11d00" },
                { "en-GB", "a6129af6e02d04c9708897f7b08f42e4410b9ec46f6056ebfe592fed1c5f20d94e35b640ab22bac709e83b5d341c11c0355bfb9ceb7e716bc428438676e11c0f" },
                { "en-US", "76c26a0d6a666c16619c3aeb56ca103a359a4f5136c96e6b13bc9bcd6c82b4057bdc0ef221146a5396b057b62474821a0289319b1e438db5a363a7cf66e4022c" },
                { "es-AR", "cea7164e9b1e3f78ffa5dfb2d03efdbde0332131a507131d49d246ed8f0ac4716a05c0384131307b481479db8521b2546ec6f92efa82d156fcfb147774a657e8" },
                { "es-ES", "0d295ff08b95f4d14243d9e71d4c8f213bb33bf9ba8d9da06f3dfe0b3b9f1cf5e86abfc51970216b95e5547d7fa67725c79ab8974dc12857df0cf52ab566f074" },
                { "et", "9dde3bc4ef7c23b70a30be1c6a6b210a68a313fbbe06cbf2c55b1956ce10e89929e1b3ca23703672cd60af547bb9287f27af36e6f7ebf12dd0b3cbe11b58eaf1" },
                { "eu", "a793a258f8486402034296e5c7482545f98dd0152f6d57fe93f1cc2e0523c8bdc07483f65d1f817469227f0ef5d513e509a35be11f5946bc7d283c864366ad8d" },
                { "fa", "ba7882e6b46b0b6edd9c6abbaec886933fcae99d88af67b03a10a024d2f782598a4e014e976b43a42ada9f66a7eceb05889d1b128c9e710e77b53815b3416b2f" },
                { "fi", "380634d83c88fe3613894b5b6a9d827f5910cca2e293d58aa7492cbaab940e6945e24a6ba02e067871c6b40aa1fa9332baa2a013bcf0d853fd64dd1572bb23bc" },
                { "fr", "0ef59322cf638deda4991513dd47f21a653c6d62878a569843a24af0e51d7652828d4f28c7b48fbb4e814d1559190894f5febc0e71b59c4d5b6049ee6a0ff39c" },
                { "fy-NL", "4b2349f97d6f191d4c9dca011b4fac1734efa9960e0c9f82cf60dcc8f98ffa7cf709f15c1792a646e455e2ad8633f65fc64b33133d6e379db538461c5ebc10d9" },
                { "ga-IE", "a46ac1ff2b99a062065d695e124f29f4d70ee92ccb5862bdcba94d5b2c1f8d71f93da06496a08a8e6e42f96a28cd09657efbc4193de4cd1db19ebd75549b65ab" },
                { "gd", "ae848fd8998d1840e6b85a38e1ce92e02a229f4814632f99c87e3983332ab6b4f34bcae72f22ae8dab8365ef4080c8710f8bd4308bcda1e454ef0532298b579c" },
                { "gl", "fde5518f63ff13313fe9e398a3a5984a64784795aa77401a87df2ab643d8208715507b537810ac2ff81757b32801fab0cab8059018e6c5c237bc1f88cdb6741f" },
                { "he", "33aba5a8d9fc38471f199b2ef9ea8fe33285873f88cfc46c44f5c3a8b01edcb2cf831a9ec711432c6cbf0a76aae13edcaa51244133fbfca93d18509ace4bb8f6" },
                { "hr", "da9f9244eb9c09039275ac97de18711c5fc20e0dc4a25281197399235fcd76295e8b878fad626384a765569d6f73435e413d91235fecd24fab8437e5faf184ee" },
                { "hsb", "cc34710f25edd790fa3fac2197905385392862c677c3c9e62365385bb99ebb35c18dab52de632df97593dd2ce98d308fc439ba191782081af3449ace10ac10ea" },
                { "hu", "b3f38754ca00560ed9014662bc6ba1ced45340001360079653267fc51f5ffc47e99a217cc469156f9e3e6571adeedc09aa71213bdb00ce560cf0ce63d207369e" },
                { "hy-AM", "51df4e8cc981aeb105260bd72dfcc6756905fed2a67931ee96a978d90474170aa64d8ef2c9b97c55614087d8fa788acfea610a786ee5261ce786d5c271113aa7" },
                { "id", "ea5ab3528ab4be37290c1b9d89c4b0940ae443c670c48fd07f01ad3e4ba4798b19861ef8a3f3917b6726fc4f7c25b7bdf95951eff84c6d58e077827d99451c00" },
                { "is", "a61bad4b67801397b962b633ff5b49a48245e4f53b318cad33c566c08da14c7fbba2fe6944e56aa29009efab64fec1f3ed0e460aae5987d83b5cb22d89336303" },
                { "it", "b7878405447af178bc127283e92a28244901bae33e0b78bfa70b6b38c11a193d2a106aec866a14f5457811c2368e1e095744ca94e99d67e23736d740e02c6efd" },
                { "ja", "c0ea6a69cfeaf0675929b7d9dd9dc6ea5aab60008238b3c8d120bbb6f6a86fa7ed0d21f93def86271c1cb2a9b0e98804e9bf0eb19c06a09aedac7b4f4a3849af" },
                { "ka", "0b7940689c7e8d9e352bd96c46f87cd036d079a90b932cc996b60e183055016592795d30180c528b753d1e11a41f6bee0d8dce14b0a5b409ee53bda6af74f879" },
                { "kab", "7c9d677a87f449c217486b7ab693da5107733cad3b775d92e00b475e95765c1350465b172e6a355e1e2a5ba420a36f010b0a2863d59e60717932187a250937b6" },
                { "kk", "59e0f6161dcb03f763a33e73194c2f9dc3b69aa70fecce7eb3eb524e7afe194614516fe9fbc042f10711f9a6cc48b911fc8af6e523513c5b8742b9b577ba3435" },
                { "ko", "6411343dff4188cce88744f56c2b6fbc2e76bca10db7f9b25de5040cc995ff6b49dcb5da3e56e21e4a04fce30875487ef83f65aa39e89f4dd5b29ac5f7072666" },
                { "lt", "1d5f720835a7a9c4faa67b20667023fa186b0eb633f2a46d54d8da131dedde3875f1ee0d90596af26e31f939c62c5ff83c726b27651e38d06993bf37a494b851" },
                { "ms", "9b3aee80217132cdf590dfe459e687cce495f90efd3338fb6738a22851b14400ee48175431e73dd8ac3e3c64048a0be5e0e39d97ab70e1c187179370f77bd37f" },
                { "nb-NO", "8e9d9a0a7fd38716c873fe1db23a0348562839e807e1d4bc4d19d45774f825298320a981de0c091babd49feb11a4c6d8eb5ec0ecae8c8681ba94ccda559805fc" },
                { "nl", "1218175afa105f216d65549ed1fe0d859def886b4f2210a111376ddb998ebb916a88296875238d1073d8fc7d9266a9481a9353c4770aad67bd3ef6e24e7e4bb5" },
                { "nn-NO", "cf64a6889bfe2be05b9741347992863076737ebb9726f3cba84e2bd265088a14f021e9395917b4533e4dbdf04c292e0b6582d77a93f6a0da037996c6e588339d" },
                { "pa-IN", "c197092b90cfa91b7e53065ee46cc19fad418b655eaefcb888a2a51e57696deb03063e8d99753303fe4db04cbd8615f37b52012fbe6fbbc6be9b1c609fb442b9" },
                { "pl", "ad4f112f4eb3f988fc846d78244098e7b3e84c977217afa7a8c41f4a3b900c79bf363ead8924ae09507188962348ed2471be646b784f716fe44e882a203db9c8" },
                { "pt-BR", "583a5f674e96952d679a5101805ca688e4195dca811636b9b2c71489e0833bc73c5fbb9bb413443fe872349ea9ff179b662ac76b580d4f49b5cccb02d46efd9e" },
                { "pt-PT", "7830ebfc6f07113662c1f6ffd252bd28b22152c73dbe1604e50286d7b5853e66508d007bab0c7f645705ffa1773fddaf9154c264c59dec06c64b1690b04ebe72" },
                { "rm", "b651a187e12517c9008e2ed0e8d3a5bb33c0b2586f410240db377b676cbf7bf451ae9e337d8ae175d1be859e323afae571664e1cfb50ba08a439fbbb3bffc439" },
                { "ro", "c8730c5c0327445adc3c9f79ec6e084f1aab983d9ea86e12373fc6f5f7e01ffc8b63bba107ad8cb20feb830091543e4f7964b12356daf074224185f5f39376cf" },
                { "ru", "b026554c466d923ca43ca5a1b0af0053c84086f91f4f7d5aa8012d91fd84a7ca0f436409483222d63dc5189c134d814c851da87742d8f672cb511c497a14c49d" },
                { "si", "5eba6c6d8fd58d2be060782e412b7fa749645f097b6400530f3a9a278f77a4479d5b5decf4089e31bf763d45ea0a3c6efbeeeca23340a272bbb90e6832596de6" },
                { "sk", "7040d8bda848c9487fe96bf0c0689d7d35c1418b62f0729528ef6e4042c51912953bc5b902398eae007361e6b0cf1a2c248f55250521d62263579912951a74f2" },
                { "sl", "9197d8ab0ebf015200027523a135ff0e550e3ce4e23aef22d6a0088412dba18781bdcd706a13cdb6d549417a30307c92aef8bb4479dae7453fbaea75609683d3" },
                { "sq", "8b0d2bf33602f0e5253040ff5afb987922d2ba24c6910217f93ab5b2d324aaf0b5a25d468be0e951e19901b4debdf5ad1639873ad594feb65875703f1b2b6c40" },
                { "sr", "ae3a2fae95cc390ae96d000fc511184d28e8affb8b813ba9667c1025bf99f5a659bbde23bf83ac46af8f6ce49f1430d05468144f05484593dfaad2855d28de46" },
                { "sv-SE", "dce00664ed2f612fde02f52a97865660445caad63f2ec4e4e4e8c08eae6a0cef811ecb7a6519ad7f4e2af2dc4a31a5c519133096da98718829bdeee969a1dfe2" },
                { "th", "45882f9003b386a1d34a1fa1ed8e6a0300459aa2a62a5dca34d6235b056229dd92c061520263403a17e44bb9bd40f53c52e2d03c8b5ef4a8fc6fe59437369e44" },
                { "tr", "e2420521b50f500628dc0757959006c69e2c5b3e73bc634eeda00e4ca54822bb1452ef0c3ada4eb57e7caa88819d7ca0bd11ec2028ce53b4f8e97f3fb2b4a378" },
                { "uk", "aa107843431186ff73fe8c8874f1fe5abed7c7e04c34695f531810d88028626e988ea6c8e196d5140bb280370ed5367246e59757cd60c5a3394575f891f48778" },
                { "uz", "37d67aee1d3f55099cf13f71a4562ee5c183701d1e6e2e3f761ee41a58ded7ba542d586d0a4a62efeb09cc808e8206ec50ab97a4858744b245d3c915b7519042" },
                { "vi", "ac72238cf9f80d1e8375f39924e793153c911d4829e6d0ea438ad89c8b698cdb3905642e18c44b1a0a3750fbe97536b760dbd6ab47c7d624c5bc1a04eea93094" },
                { "zh-CN", "40b51962dcf82131c80960c3eb4ad0f693db57763b2e2f7450b70fd08833f693d7b61eb31556d66c3d40cf704fca774ca85a3c81080f9cff2f9aea0d1c3ba233" },
                { "zh-TW", "d8961068b95705309ea1b5cb65e6beb168c3047ef6fdea2509ec3522b25f2a4334eacb6020b2671f7411d850e65ffcfe54a0f371f1edadc265b184157e17712a" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            const string version = "78.9.1";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird [0-9]+\\.[0-9]+(\\.[0-9]+)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird [0-9]+\\.[0-9]+(\\.[0-9]+)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win32/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win64/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma"));
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "thunderbird-" + languageCode.ToLower(), "thunderbird" };
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30_000 ms / 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                Regex reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;
                
                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Thunderbird version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksum of the newer version.
        /// </summary>
        /// <returns>Returns a string containing the checksum, if successfull.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/thunderbird/releases/78.7.1/SHA512SUMS
             * Common lines look like
             * "69d11924...7eff  win32/en-GB/Thunderbird Setup 45.7.1.exe"
             * for the 32 bit installer, and like
             * "1428e70c...fb3c  win64/en-GB/Thunderbird Setup 78.7.1.exe"
             * for the 64 bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using
            // look for line with the correct language code and version
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value.Substring(0, 128),
                matchChecksum64Bit.Value.Substring(0, 128)
            };
        }


        /// <summary>
        /// Indicates whether or not the method searchForNewer() is implemented.
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
            logger.Debug("Searching for newer version of Thunderbird (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if (null == newerChecksums || newerChecksums.Length != 2
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
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
            return new List<string>(1)
            {
                "thunderbird"
            };
        }


        /// <summary>
        /// Determines whether or not a separate process must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns true, if a separate process returned by
        /// preUpdateProcess() needs to run in preparation of the update.
        /// Returns false, if not. Calling preUpdateProcess() may throw an
        /// exception in the later case.</returns>
        public override bool needsPreUpdateProcess(DetectedSoftware detected)
        {
            return true;
        }


        /// <summary>
        /// Returns a process that must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a Process ready to start that should be run before
        /// the update. May return null or may throw, if needsPreUpdateProcess()
        /// returned false.</returns>
        public override List<Process> preUpdateProcess(DetectedSoftware detected)
        {
            if (string.IsNullOrWhiteSpace(detected.installPath))
                return null;
            var processes = new List<Process>();
            // Uninstall previous version to avoid having two Thunderbird entries in control panel.
            var proc = new Process();
            proc.StartInfo.FileName = Path.Combine(detected.installPath, "uninstall", "helper.exe");
            proc.StartInfo.Arguments = "/SILENT";
            processes.Add(proc);
            return processes;
        }


        /// <summary>
        /// language code for the Thunderbird version
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

    } // class
} // namespace
