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
        private const string currentVersion = "136.0b5";


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
            // https://ftp.mozilla.org/pub/devedition/releases/136.0b5/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "53347ab5838adeff9a79ed221bea9f3da86169b9a07251a88e62fb379e5c0f338229b382cf9e88110f00f153f89d7aa26fe4dc0ac1075a9ebf352dd0dbf68bc6" },
                { "af", "466a3687c5a9e907cd12c7f98b5f3fd7be057736d654873e1c6ba3924f34eb4e143930c57ae20c3554324857c9e808b13da92007070353600be979bdd4c4538a" },
                { "an", "f5e63a29345931974c62b397ec0486fa2f8fe605f8b3016267681b39b83148e9286d4baf119bd5beead0d8b515c34b380ca7034ef831388b027693a680eda2fc" },
                { "ar", "1e2095d89faab0898d327f1ae193c3ccf08978742de1c035458d05787a35ef47743dbe941f2af0b09ca0ec54da18598790977a4f91ad56545b6dd29e1ef59733" },
                { "ast", "210f2022f3727de33b57ca30e22c329551ce25ddde24fa0e464bd8f9e9b3abfdfc82deee33a055fdd33d28ea3aa9ca5611a9a828a8fb08dc0053378e23c00c52" },
                { "az", "ada9ad144564fb5ed1a2477f1532539c76f4f66418d8499aa4ce3c0553b5e8f41bc3b6dc9d810ca140a16b7d652d0562ff38a4df5c6bd1b612455cb4452aec4f" },
                { "be", "ca935056fed7a33768e470ce3997e2b2416c18dd1c2b006cf2d326c17d08547875739188ed5773dba5fac9e6db81d7f4311ab7cd0aa4d6d4e690ce98421b69f8" },
                { "bg", "29e73a22397cdb5df3ca5fe22bf0268c9190683c4df574f3d31faba3ca2314c1a6187c689a2be46e04bf132af82fd0317039117ccbb0981eb4ce54b9a7c8ecdf" },
                { "bn", "8e9c2d2f3a81c47e9f64a80c179e18394d6a89a5fa883fb212316290b1c9f7949eecd066b06a338e0eebf81cac6aeb2484748e3b246c9e7a79770832238bfd84" },
                { "br", "1ba0bc1e07f5fd8b5850915bbdeea1205bec76378af91d00f9ce1a5df4f4b057ce5b94b54f7f8b1817b2f45094e3b1e9f929f663cc335cf40bb81d90d8d1cc5b" },
                { "bs", "8eddd9d40e0182aa7db754043b773e5fc03d3f93777e77e34808771224b3cfe4e13ab05cd5b8a4714be6843c92ba0e169e76e2981b0493863d45c6a5621655a1" },
                { "ca", "2d21f4795018eb8a78f0902c608d7e4c434fa1b845bb702056f5c0d0d202cb7900d87e051c8be5ba7d5a6b1a5f81bf327d3c6db1bdffaa75f8513eb82c4e952b" },
                { "cak", "a356e8f4cfee64363c93ace898aedb6b1652be8f7368dd85e493d778bbc3a0f59a66ae69c27f800887bec8c01a977924e682cd312fda6e2ec9435ebd5398b7ee" },
                { "cs", "9342c947f716e8599fa13c81b6f74007129660d0c79c207b9a0a99cd9e9e5e342adf05b2899135dd824b68167c12148eaa3d79a1a406e4d9a73c92d261923bc2" },
                { "cy", "b6a85cc94fc63c53a435968aeda6b799a1a189aa4b77fea22bd9c82f777ab6c3a9be81b140fb96823c077a20c9c33d540840b9a2680493d2459ea9bd956495bc" },
                { "da", "792c91a0fb7d8125a3742230749a2a199ec5724be3092d34a704dc453f621ccafbbb68b72d133ff4441ad0b3048235076770491c0fa34093583481f4970b534c" },
                { "de", "ebd2cd1db4c93c0fb63948329eaf66681f9487469127aac275bd9da952ded01ea2145109efd5f3d9e03ccc8acb45fac1378aaaa9d19e2e5e63ee6b27edf7b219" },
                { "dsb", "440925a4310bf84c74b40a14fc9af612302a1fbf37e4d3a461748606bc4c45a32b5acf57ad31a5a266152573655c0291368a7dd15520933b14baa6926128437e" },
                { "el", "68663020202f52ea0cc6440d88a245f486238ce2d29ecf2e6c0e893a464693092b334719786b0dc57350b417204aed2031495b3c09cc3ff9134944c1e0b01e52" },
                { "en-CA", "227703619ae5185018b7be3144aa810e321ff11c167c57c9864b3c01d26b06b5a3dc98f938a0a710cfd96d8c06b69227bc3897acd3cc6c682ad01cee017e3215" },
                { "en-GB", "d1c8b9b0bc74b799cd9632dd08f54ed351dda896875fa0fe72cdca88aa7064910d4a98530f2dd7a4ed7dd9e538def51c52b3fc64852df31592a4e018f1681dd7" },
                { "en-US", "f7d7e8dc5fa891359c8e9c005266d9aa25148c59d71e351890b2168c8b43a430f7e74f5a693dff240382a1b866549a65b38c2511ada7b533dca2b26ee8cf8483" },
                { "eo", "16f069e3d77fbf0019a28b490d74051b12ce8204aabe37edfedcfbb2c677793df65f10307787dad756af28df12980d24e99a8eb9448a470a9c060dbdf4ef6d4f" },
                { "es-AR", "9d0a06ebb5a5306e2d7fbefb0905c240ae19352d0f90a1f7da6c6e29e76eac7315c56d1520df7006a6594342da80f2fcc081bf35aab3ea6a17bb11c7b2c358d8" },
                { "es-CL", "cdb63f2d1e74f55fa847635d953c227e5c331fe4de7e06dea2756e45db8759a1d77cff373e8ffcb4e607706bcfcf1ad6ce3a237d1d0c3817e70f08f697461c2d" },
                { "es-ES", "a008cf1ef84825a8f1e6253e7f9d93d806234106c33ae91d99f9842e808e7ad6ebbace0be7c1321ecf77fb7f1ea1e811583fa1b9dee74fc2801ef5340db59fdb" },
                { "es-MX", "9d4cd9d2b8539b433e7e17fc33be22bef25d459b3fa841127e926c9f08232326f78f570e6ef1511780bb5da98b54687eda4e7405257c52e02dfa44ad30712f5f" },
                { "et", "f92822f30753a3efe8e5f1e52975d12ac9ca0227b016e044ec1937f7edb3707b8eb0c07fba384c88ea39c1c5ef6b44e08c18efa2b021a28f922ae21ab84776d2" },
                { "eu", "1fddb175b3ded849dd6ec506e53a9c8360463290a095eb8cd602487a66e50324e61ae947c4d5fdb79d7f996ba3fd2d921b2a1bb5d93bc8968ad1495b2133caac" },
                { "fa", "e81bb5a1703e46f9f4e96837df4e656a140674bc16c8510686676f8ee2906f0bacec8beed887883ed87c463970531ac3f385605514d339c726b8725cdc1b5504" },
                { "ff", "34effbd661760171063bc1be3210fd631f5158d4f06e7bfc631eb9ee4c27bdca497fe1fb9cd78db4cfbe76b5029307027f1392dc00d6c857099c2d55d77beb4b" },
                { "fi", "e5e9af9cfa46914f8aa2c16301a8f4e5882bfd900d51cf88fa791acc13233ab5a9214cda067e3625dedef8b4d3a944b45dcc1d1d5776e692b0b5e273aa3eeaf2" },
                { "fr", "3908f3d359620d33dcfc4fd453fb6c7f83b2e577c7ab56ba694606e5521a23c5fb33cbc1126b373becf704cd2a506d0b469a9183185cf58962c8f2f317e8b2e4" },
                { "fur", "badc8c8c87ee09fcb46e6d718fe97cf874342c6ebbc24b444e2728b6e6d1e137be4e2a0a472d56c4485da700f81763a3c53ce8d9845026fb0382ca4987343811" },
                { "fy-NL", "545947b116e4a999b296457fa5aa716431516a92607cefc6074c97e3cc20b1ce1102687520042ce769e4ba89cf68e501eb13bb270ad2cc884961145faf70fbcf" },
                { "ga-IE", "6b9e4fa4b409eee953fc217da13cbd2e1cf58b98899993e68ed7b3480bb79384316e6a8a4c6f7595e18279f1fc73c9422256dadb5484ddb6115e79f1da0ec751" },
                { "gd", "1ab13d8147e4b8c0b08ef0591239506432ecb27d9fb38843a1192370dcf1b08106ee833c62fc8470ec6aa7a7b4b05f554911ec1bc3e7cfc4ea09e7c97445b4a6" },
                { "gl", "c64888c9f743a545115251e34a75446022b8f756e5e87b42ceb2c33d5938b550c65c6f151519f01f4fb5300fc10f8cfd83e09d59f979a50b2772e8a19560df63" },
                { "gn", "72d9b26520ee5937d4a4ad0ca3395cc0f7133df14cca7dfc87fd1375be45f168266f1fd37f4bd4d74848c493459c8977d569a7dcd11ba91b10f1a184dc8d5f96" },
                { "gu-IN", "23464ec2ad5077b577adbaf71ae5e2611de37ba0f9bed03b2cdaff3e96c89bc5e41d1cffec565c9cec29022e4e7043ca485acf359b7592c16383b7ffd6a73d5a" },
                { "he", "da53355b0f7f51062f3e7553d27089fe70af9f8dbe468a02edbfd7b5dfc6143845ca924219b29303fa1d4cee1eeece105c5bc0318e8864175c49905a7d106f18" },
                { "hi-IN", "d305f1e9d03502edb520dbf58995a1fe9ad5f6ba03d6f6909a831083e60d8d52e605bce3ce2d38ee26719e04f3c90193cd5a864b4e0f789082644f32f29fe3e5" },
                { "hr", "bbd86df0a81e39101945e7099ec850c63c11a04b470e2fb9fcccd0456a77bcbf7c910bf6d1629daa191adc90cdc699c79682ca29c51d4ded7ac8f97176e792ce" },
                { "hsb", "9090da87da73174dd94b842f3e7a7b6b33c22e976b6c6c3d27d17e7b63a682e62e07e1c30b883ea95705d09bf28bfa1f18d9630652ad9758efe04bf14dc08249" },
                { "hu", "684afbfb8c851eebfa25dab0447a878ee8efbe958f45c51f21384dca782764a1548eb7b799e64e06efb1f9dda711f5b698886ec8f7159f3358eacaaf086f850a" },
                { "hy-AM", "d4e7cea700fcb35b0b0d5fbf65476213f2f5a39592b721f143aac04200d6ad59fb7e366789e96052b117472797bd1dd38efcdc9034f74da597e79ccbe85dc072" },
                { "ia", "9aab17a50872022b137589b4438036db7eb4a0f2e2949a80c5da1a198b02f1c897c4131f8abd7407c8a6dd17a67f671a467a57d5568921b09d1328213c0b6da4" },
                { "id", "a38c8c12d7c211013462aa3a55d2750cc702e1c1ca625dfa860b901e9aae429a9bb4bd7c21a492b77c04edd809482ac83c9004f01e02c31c9ffb756161411b39" },
                { "is", "2cdd08af759031b81fdf7af7020b5cf0cfda699c05d969e985c7a5eec378bb827c7b07c59eb9246e861a29f6b7c9efc7a0299dde85404cfd4f0b217bd652e04d" },
                { "it", "71dc71ea84c6b073161be6b953244c3670aac19c7bad6027ae296b2ee47129be10755ec27c6ef20e55c8a519462b6a3f5bcae0dec1d1a1cf1d524c0851895cb9" },
                { "ja", "1b412ed8a24a56d477ad79e343e6b6e17f8cd78852bc1d0d59ddb6b433e224b5d35f2e6b52373997b875cf0d8b7dc40766b37e4f80dd9e3cc52ed1609a2df231" },
                { "ka", "d2ba31b2c36a5561de4e7521a8e02800233112e4a6c659cd62e0626f3139b2f8df0bcbb7f51f442e396fe516f5c4f8644639c9dbb425ce822af205afe3a96b37" },
                { "kab", "c3a4eb8ad6a4c5843d3470e89f2bd6ae52dc6f72fc0f066ef37bc56c5e04d0359466a1be3814b304d56688ef1cf6cfcff5796b8d9c041bcb729fc30a5c799204" },
                { "kk", "bf501bec6871e1be702eb0e5bf3165008892bf5301e6d3f9a8454809378def15744a69e2940492dc66126ca5ace61aa3c55b8499b9b54394709dd3f0469c037e" },
                { "km", "91334597fa18f802cd5220dcd6851867b4c2114465af3b6465e3dab8bdbf70c9af6409803b069fdb76e52b6c571f41196e214bf274da2aa3ad5f3b7056f8fd4e" },
                { "kn", "f0b69994ea1684f2f5fafea9f2036a601edbd38a094ff88ee8f2bf76782745871ca90d93da9e1f6c87c1ebd9988f9b1caa2342e676e674e74edb32d69b42a5a8" },
                { "ko", "3a143c03af9eb8c202572aa1712b01cf237f1a0e0d1ed4058622238119d52fbd3e119149801c7ce12a737072ba79661330acff358340890c34a6905982333633" },
                { "lij", "edea38b058759c2ffba3f23fc47c3adf02c6d8d56195f1e7f2e770caafc9862243e4868b0054f744da6b8ff55ccba0c3af90ce2d3a8d9386987eb2f689deb77b" },
                { "lt", "48a53b6d5bd7cfd88ce2aa0a51f380c97a2c1e87234c0025e77c37c2b311f53c2ad8f981d81a5d402a511e709619497ffd88b6cf959b7a78a643dc36e8409819" },
                { "lv", "99b7b82b0d37c289ff0750dd6f9d6e82adcba67d337af078d773c5380347dd10ba6dcdc3dc9877934fdfb3b0875c86982031f273a80cc6b5ef2f8392c9456498" },
                { "mk", "1688738d7eaceb60fd9b29c63b66b7c021fedf4306ca4d5cfd08d842895aea2d6cceb2f3aea358168eeec746a26205bc4ec29505200e606911d92a712bb70e22" },
                { "mr", "57a66eec0105226ab33a51d62466e6b2d553aa546ca9ccf749174c8f097205afbb13b53dd7fe653ed81dab2740cc16b5eb467d3124712033ec9f5ab0ec4714d9" },
                { "ms", "0542c854993e1c1636cedbfde157eb2e33f6e6f3bae695854f325fff20dc3e0816fe5e843e898dda55ec8f6ece3dce999f2397ea61466becd3b3e23a2974b86a" },
                { "my", "94e5f14f1625616d504465d675437513115659f69375e420b2d3dd86b6e20fcd14091dcfbb685cc88ca0634e14c6bb9d159fb1912ae0ca72f735c8d523631509" },
                { "nb-NO", "e49bac8b847d587a83062eed98dc56a9284ed99014d1fb23eab1a93ae24b90c71152d9dca846a62e68b08782d90888b85950e17cbf96e61292b8ebb25ee0bde8" },
                { "ne-NP", "fe0269f6c7ea9856012d41e8d8343e581a880bec764f4e6683309e9da7b5e74646bbfc6a8d218d6b92a17dbc6e9517667f177dcce35e6a1a643401f2b5d05398" },
                { "nl", "718058ed4f829778d822a827699b02aa088de9ec1a0d24e787076a7062b699e1cb5a42c85dd26e7f0fa061f240e6ddcec6933f13d550f595ccfa94ce2ecba3c9" },
                { "nn-NO", "d68460be75df44e63df170780fa19f142008769972fbe4a022b3d46043ae10168ccfa02ded2d479fdaa38ad113fddcc216c17f9862816a13b35573e5b30f4073" },
                { "oc", "70d3ff04bc288d2fefe4642da0797d22989b9d2b2cf0983a080a1667dc5bba2312e9e8d364db89f4ef02f4b66137b20c2e68d6a2e2e6abbe8b0e596eeaf39331" },
                { "pa-IN", "c8608b91120457fea7f3393be72d2a206bb7ab23fb7d1b7c712e6f665895167307924a3cf46ccae014b3e979a357cec4e96ba8eec19824b1ddc4d77799fd6864" },
                { "pl", "21d57b7e67482d52202f2f981d087da7b8f181938ce5895e40a7d9a4a2e7f18a66d84920b90fa89da00a5b6e50d376492e0866c8ae3daa5d75a1bcf9f0810f44" },
                { "pt-BR", "26a3f502da31f6c9f0e4a186b96e5565d13f6946244157c973ae30abfff6aeb07e5d227a3920075a16a81c64b2a0a8c7971893f3d0414f5d544b18d21a8d051c" },
                { "pt-PT", "461546ca51798149195a375685a479c5d840815b7128a991a2f2deb60f9338f8a94ad2893e7eeddd0ce9aea7e7856609e7dc6236201a4adfa4fcf29f53d6620f" },
                { "rm", "944ba5cd8c56a322eb93e1a8f72b2c36a2b7e9e0dad3b615c1859d54aaa6a0085aac5b443d585b0fa8b3cf449a417a0d21667e0f1cc6de438b05ea2b11c2cb33" },
                { "ro", "f05cc8ab75a78327fec3a31bd031417302c19217d53348f1118db05482f70feb64ef5e3800f5a357047af576ac065761bdbe6f201bde7774fe62b33679b18ea4" },
                { "ru", "dfc19f8fc5cebb29761c6f62066e8af1bed52d72665d6bb4615aab7bb679d9265e7085e0080372dad60ba8da418e99e702dc6e2b2e97d3a3f4b3980abf4f8bb5" },
                { "sat", "3c1e9e13c9598eb13fac742596790af63868c2a6cd23680c18ef1b9c69225806db2835066b00145dd460093296a3a5437efea1b4f54788b03eb5a1522b26d4b0" },
                { "sc", "cd0e77fc45e338dab8917d0a046d2da5e695e2d8491c2e62b1dfbfe404f8ef066ff724a29fe4f0af7838a1c596633f0464c047d6b8e937e0d274c88eb6563356" },
                { "sco", "4f337d89933dab9cb374714e7a24f954afae305eaa170b84bc63c71d1968541363e95daa2bb399b4a9ed3a0d7d7f4d1c518e61324eafb47d8d741d9f4214acce" },
                { "si", "23e891f80639ccb6746a4f5faab2928d632f069a864aab39f096c44b9ad1a47ba261f96ef6e35ee35594cdf0d8c6b9d3dc0cc1bceb07702566f3de4709db01f6" },
                { "sk", "a38e1a4769bb29fdf5015315f5075a92a1f878f10a9ba07c1848784642dfbdf3c03741017ec3442cac3420e2246839d8f3f31eaade207f0b08d9cee940eb06b2" },
                { "skr", "31202586b60f90d212637da0894a7d3037963be75afcb2794122bda9fcdcfe797628cd8446e36a80b99a0d1ea99c6d6b0e3e33f847d61ba6971cf036d2da9586" },
                { "sl", "7ea22c098521b1ad6a2fa212b4ee5e49be1d8a23c1dc3281104608f9d15c6e7b50decaf8e30733fe2fe1573b8f6ee9c4c8c4f2cf1059e3c6d7b41eafdbf3840e" },
                { "son", "dd3f916068a0b68277dc43380773ea26ecb24255b25f6fd4b156c2af996f24e9b53d53b50ee897d5cb38d4be460d4a00fdf2977e963567ccc35430229effed09" },
                { "sq", "ca0f6badd22fccd51c696742a2dd1ec84e42044f3c8ca99973012ecde6f9bf42a45425fa44fa1d51ba0138b06b01696991fad031d92f2f3ac200e24f70f9be16" },
                { "sr", "7899fc3f14e1dbeda3ba021626e50d04b3fcea0b56f785e6ac11edd3ee1540c11b79beb7c096caf0a425858999c1bd17d8011119401742f1eb05c1918b179d82" },
                { "sv-SE", "ab7a6b0f18d52fcca2d24f318d21e125ffb9a4cc06ae3ffa9dcf7d98586f4013b161285606ca3431219dbdf09e3f6798c5f75119fc0aff2a69c0e5926226052c" },
                { "szl", "ccf8e3ae3d5e8e073668988ca4c6601aeba23188ed822246c2fd69aabed56dfe40d31c467a33e0203d09238a6ea319c27ec03f6ae4cd539d782a091f2a671b39" },
                { "ta", "4c15e538f3a2ba8aea277969e6bc456c4e16e26c7b388e6544896fda4f6f7dc67b85b4b68bce4b0dc99a6c24fcfeeade50aea38e0c4132639eeb3c75fb16dc8a" },
                { "te", "41c2b6201bcfe531a834e0b09adfac8d19bc277449033f98a6239142220f1a91ee432f6cc6e009fd553824be9c80eefa6c1698d331371d280c2c2e883df42473" },
                { "tg", "5d704d84e05871b7f15354ee4618a26b0a74c20552e5412ebfe5e5d13bd6981e8060d5314533d86bea30759f0268c851684c6d252c12810d5797a3ae0bbed32a" },
                { "th", "b4a0e99049e99785828e43df1be4d2b43173121a97c61d8db9247e61eccc00f8f33fc8364132d905abe84602f90860e5f3cd47a2459e3927bb750e2292fe7762" },
                { "tl", "907285b5d8a3c356970912dd4a2df4979effd11354b41e72650efcd818cd5ab1a858c09524a7ad4147bfbb9ddb03d9aafbe89f772bb511f12cb9002231663c34" },
                { "tr", "22f535b52ef82d5d27a0b7032b95e8c9c791d05c98b201016433aec3fbae1857cd35796ce27e4a02c24f28cff1497329df089e26b72551e8fea1f2593f3241ca" },
                { "trs", "0edd53c3a1e1e879eb23bdf43ff4686f299f2590fdeedfb516042342a78fccf5d645e5d7e67a5952a82f7e08ceaa1cee514157817141570140bc0a9a7e028056" },
                { "uk", "a72c3b8a4e1215b12ff2156436d9b488fac30e83e11ac681f7bfe1b6c4079dd16e0b22c58d8407ea26738c0bddd17e7cb3e76eaa1c908f116e9c435f36fc5044" },
                { "ur", "72d5f63289af383b14adaf489d3e5dc0cd18ea38468c735d7bf64a37f43f38e92a1fe05bee54d642d33f288c61db8ee6659a38610f21efd77690976902c6e20b" },
                { "uz", "113eb14df1ee782a7ba829260bfdc784e423eafea4d168e8462340aebb4ff842390916940377fc9cd2a0518f9ccd23ed66b84ec4ced3c38c3ac0fd73c8163b3c" },
                { "vi", "300a9eef13bdb9e2138d44b5aed341ccd25952d834c0ad519b52869b51b5da878646a97b8aa865219f40b547ba165d2f74a7f9ba01d741cf9e1946b0fd443112" },
                { "xh", "0f76cc41578a4e7c484abafb061389b4e464d452ef8c9a3f421beff77fbce307d7eaf10de1c62a4dcaa2d72317e353d9748372320e409469667cbe2bd7a9dd08" },
                { "zh-CN", "71f6d6fc496b42370563fb9d96dcbb932ce85d67ecad0ec7bf6b955b06b7e24bb03229e4adf372d1ad937f517043843068954cfa898baf39079bb60f593e470d" },
                { "zh-TW", "222208f5283c993c6a63ed88bbe2dcb3bbab6752c6b9cb66aeada743718b08fba5fe0121268ebff482a180011f31cc41f77faac9e5de1c03ec18cee6f488ab38" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/136.0b5/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "dbc9f3536359c22f0bf4e9dc6ff87e24eec5964a3c13227ac7a7257cc839cf1211066eb1456395900f20a3ae423b2d026f1d7bee4c62976c94b90223a26dc82c" },
                { "af", "3977f1cb43628eb8289952d2827d173affec5c5a717b13499dbd4332ed5d5c1fd8fa2e69499abeff794c4e805e442ebd3b4f0d9734dd2c67c6d0376aec3696bd" },
                { "an", "f4e8d067ac3b4b7f27793e508568769706809814aaf6f048a4e1f67142bcad6dc40a2b3e8338029a73e2d124adfc84a8439dc532249652464cc4139b198f803a" },
                { "ar", "6b361b0bfb36f6d35440f1246145d393a9bdedcc04b08af2331605f625be61f1fed0bf126b086d5d6aac8539276466f708fd2d8cbc809ccf9b41bbb66f78f49e" },
                { "ast", "d4156c70c29f065cd2fb693bbfad7bf35394603ead6cb58ca8db5cb10249d18e1eeaeafd599689b0927ee244489abc24a242d31a44ee375c29940b3c412ce9a7" },
                { "az", "8c6cfe0ddd99f685dc3a34025e09a048cfccdb9df65f70c15786ab55a0ca6d9c05462822b811e2d96260256c1ebb2271fb93686d273048f42c3d7f45b4c15e19" },
                { "be", "8ad2e8a3b9b5799d3c0720fe991082edf1c67de1d5633ba509c135c215345de0d86733777feb3c25af93ce949538d3d74307c87f94d983150b49f479fcf6c2f0" },
                { "bg", "4184fc46782b27673ee6d9e01e188a04236424ae0b6378498ce38b2aa682a7ab2026a0773fe6b2381b40b2aa3b9ee46712eb4f0dc7e58942074e266b8b0778bf" },
                { "bn", "42e93090db2005f0672e6949d186b6c751a0ce438448b91ad2ee84844fd91c5ee24cb4eaf95bde8cfbdaaaaec570ef629c9b471523b17463ee928b75d4f65a00" },
                { "br", "04ee30807d1e67c8063a1db1d389ca7a1887d1bcad6120ff1b8d416d6d03480f852c95e2ec97183e861a0ffa05c2a924a745daa6eaec4cc03b2cadbc5909d55c" },
                { "bs", "da859724e9ee3f548fa80a9c9c7150a158bdc04f6b4e26b124adc0afbc643fa2a19631be2666353b5e992bf926ca3379866df0dc5fd95659eb07de901fc37c32" },
                { "ca", "5f995d5b87738882840a6788aef2d376e8cfaa2b2ea68cbcefb3f979a0b1faebc93c9fb9f1fe6f41b913974d7b9de37d0e6c3039b1b368383511571631131369" },
                { "cak", "07ac79bf6e4a20377da16f4ae6e349e43cc4f4bf0728c80638521811c7ebebb214fcc2f13f8428a237c891240986a6a41240249da392d809f15ad740d71c2b23" },
                { "cs", "dce5680f312272dc37d07587dd00443690401dba9e12c861658ace941d0575d948e68619eee306917a5512542bce605267e960fb1f0960e0cc43e4e9cd59beb7" },
                { "cy", "67b2c00c43a55897d689f80b50e10906fcb71ad19b128004dfc5182a4557066aabe0d8e4d88138dce762c7c91233bb4a9b7a2e0227a1d0fcdb42835b23255a67" },
                { "da", "98342aeeff3056f32faacbd7d664c4877e1f83f7e4c7a8925e37acc3dfcf2f9752e8c1e3b9e1dc9a74ceba67179b187f8545a5b7e90811e76af8ea55caa2050a" },
                { "de", "d125c80e5d1ad1c501f3e4159762cfc985031480cda1447fd50ed224d93ca056b7e1259dba9143e955e49da9e8319374a6200c4d8ea0f4898fc766de0427db35" },
                { "dsb", "da65ce9fd6b3fe9612f47846a6f90468b00dc5e108958f15e762e78dc9e0478f5451dc5d0fbc6539885ecd8a50bb9f74060c3bf4bc7682be80f653bec8bfeecb" },
                { "el", "453e8fd16f1fb7bd37915764bcf18a17342079b4ddef74136dba8436510f2a1113b3820df86d681a4e9096cc4c3e873cb94da2cf4096e284a8396720fbd3bde9" },
                { "en-CA", "0b4ecb9f016e277e4ba81034c0e2a876ac6dc7f97cbdbd18907a2071f7c0b3c768cbb1a8bf7ccd75d0978ed5e90cd85aec27f35818f49735195ea452d4b395f4" },
                { "en-GB", "24fc28969c3dc5fdf53cd7f186aa7f29b559724a6eac40ac5d20d960511a413dd6b677d5145796095e7b321f41bdfa5cbb004bbb722c68b9387b0d0f6119084e" },
                { "en-US", "394d780f1dde686b7902ac2b55179096a5b44a540e47e170a97efec60882f7a0e62cf22d45ffe0f61e0cd2b0a6717f8ad6267345ce1d8c52c000544364d7e64b" },
                { "eo", "f8108ee8b717ebc5b3f59ce968e7f4bb346089c8d8dc73207db6c8fa7411006805dc1014e86f46dc50ad57fa52496c9f9248ae1939f9620dc6c2188bd18de599" },
                { "es-AR", "e45628ec79a48e3bbbbff41673b18812439703d16e1804d735c60d50fe231beb19c5206a0d1a85afdf94d85f6538f19cc164907087cbf04498724243d5eb59a5" },
                { "es-CL", "93f67b712a0d8d643033ed35cc038d22e1ef4912d4d32fab1d46984421c17fb119d48c2114ae2158eb33211ef5b536e5b8273f899dbe40f5c7100912aded4606" },
                { "es-ES", "bc1287e748586f68fc5f0073bfe99e0cedeac581da0c9f288fe01e0e9050327812de9277a39c03fd79f9df08a753ac1332c054b6c7111e049313e50f351ce340" },
                { "es-MX", "ec3283cfceeaf0225577237eeb04606466069ee9cea54ce59e2ea538aa957e1788bbb7cd9c07a81b51dd0ba34f058c0a554aea7ad5194089b809b8f9d35aa510" },
                { "et", "69f64457b7926f8d7bb4f485c615d4c75a30a1d697c6c33f6f8a2816102aecd1bafd139423ad719c32c87e2891f6b0e82b9a679d98f3e0f9b6bfd094837b4511" },
                { "eu", "25f873ff13beabdabc7a7acdf808c87a95b4ac520c290c74fbacc074b38123f9b3d278e90a4744d116ada3989f1c08fc400809ff9d7cadeb148c1e65b5e9c383" },
                { "fa", "a28085cb9ea90b2cf54ca3400b1c81d7dc3af9f6805b87afb9c8b147b6050cfcd2d9c0a6d892e702d2f2b9455254c8536b41e209affdc5fdbbc2ae014378361d" },
                { "ff", "aaf7c4b3bdccdb1c2fd0aab51b7aa1dbeb7d4a62dd8dde25083c9c134399bf2892bba4eea18e076dd60c02653ce3b5247629ed1896c1e11d5aed9c6e828a6bd5" },
                { "fi", "7ff2a58d54809158be32d46cbe2e2c82e5aac2ce5d0a8e8c420ee6e4ede24fd8b2df8fe39b03bf55dbc9cf28a05ec1875e80f49b00d903b81a5bbc43dcbb8265" },
                { "fr", "35e4f0c3ff29ae2c86c04269b1f9b6460db16c2e40741dfd8998bf30be45bf16bf51564e0ddce5d2b6d2a99a3732ac72d2c04410ff57b8885395ca2ac44a8ef9" },
                { "fur", "d74f967635e2ff5b5b858e01ca31934e3963265f6b0ab43384fcfa4b4bcbe7f0bb09992d68d9529099a7401d9f3c5637e1c59109255cc669000473a3f951f46e" },
                { "fy-NL", "c1ebfe8b03e7d9e1e09ba7cf13a86531a6083d923b8b15f56e81b71f0eee7e4c3c5d98f31fee4bc0c04ed733146d574653541e550e8eaa4d8fbaebab16013a41" },
                { "ga-IE", "c338fc5bf7ccdb11eb81f74e5486803a975109449fbac778b8267002fc9a1e8699439d44c606dd3af0fcced0daf04df8111dc3a6320845c911b081137c7b7cb7" },
                { "gd", "1df9f3349ab77ea3895d2a399a8fe3f771ee4be2d49543519f2e3b354e79f0a1037689a4e6aebeef5c231bd438c77f107ac4fcfc43881d81f7198a6db31813bb" },
                { "gl", "e4f1fe50c755efae07d769bee94f4f464f09df36643f2317d6aca2c3455b31475a2a78762d43204698edcc53f9cfad1c5f29663a566919383c556b28d1432d15" },
                { "gn", "fb90eba24e596a5376421a76729cf26ec6c12c695c0a6080fb447e4930e725a54cb866c78c7e9f99b8dcd0c031bad3701f58468b88983cc7c565ed2ab2c6cd1c" },
                { "gu-IN", "fdde49ccce815d68457a3c05964019986ed647c5bfe100459a62da01e2fd55c7e7cd8b6c11cf4dd6552527d2f3dc444054202995d57713a0ab476b98833dae76" },
                { "he", "ef0f52b8ff223621de150c7e872df6bf6f99cce06ca44d82c725258482fcde730c360f6e2e9b0e4a63371844d73e1417518e425b42b3a7c44477147665d71beb" },
                { "hi-IN", "8d762f7e36962ba65d432f34bf8d6cdab8d3daf2755ab393d379f6ef32b2cc5fd3c0f12983eaceb091fef8926eac513340e786a72b7609b53ffbd4273d4db758" },
                { "hr", "2fc079376dcfa9006e9d69fc39aba22e6c1c41adf01b409389f0ffcd984c572af1926cf660a974c7bf244fa43ad634fa2ed572fc63994e39c085abc00879726c" },
                { "hsb", "75c1d3bd1c018a984396f850919d2cc3bc022d83991c8de46efb73d0eb0e817d8f7b4987530b12f1edd59598fd15bda0431bcb4a8a4d52523c29e4f13836e05c" },
                { "hu", "d7357fff6dd1ebc683aa619f82c04a06d8aa61d39b9eac025e17dad7bbc07e1608e4009f918b6219266659dd4fdb5deb45bd979e6697b13ff69df227ea88a8f4" },
                { "hy-AM", "21e8210cc3ee831052118f83d4c3cdd3eb2275a124dd56f7c621ce7dfb9748fc363c125734cb424c9d2a32379390bf73fc519b228757b0573522fe83d1c2a514" },
                { "ia", "bc23fc4c3972725d0976b10d847525456b06ecd3a143e805d1c9a8a025455dded147d7506fd441bf95b94a995ac74c7bd13722636a672b14c481f718f059e6ca" },
                { "id", "8f13ccac4d75b746fc349e5b513a423224ef4f96c38e019d8fb664c2b12f41f62eb4df37557ddd8549e8ccf912ce94abe2efef39ccfd9c7c06672baf00ab81a9" },
                { "is", "058e1e906d91eed339856f7dabf786728e33140d06934aba405bc6c206844ce5e61b3d48026009f25987117e314af6e684cfefb390759280b03ed871d142cb9f" },
                { "it", "a3cb7aaf5927d810d78c6717d10ead43f3b169cad19054b7952ff0184dcd818bd93ba8d18f70b5ec5640ad104e21b1562fa900b6e0061fe0c8ea60205d5448e1" },
                { "ja", "e73d1b3de9abcbad2c61e7d9be1cd49a451780c8893723168324fa283591bc5104a52885ed84a873245086e5a034b7706dbf0390988da358c52bc34bfa86870e" },
                { "ka", "74177749e263b8532214f3faa96e2f208a90415136b3604e12017bbf2d7f38ef9feceb29a971c2c1f7b89d6c2100be35cbedd87d4684112589f654869237a720" },
                { "kab", "4fe044cdee1c36657d1731c83073e4172c381008a0a77a99098270513a337d466fbaf1b48739a9d20a5deffd5661a2360749c5a84156a07065073fd02608778d" },
                { "kk", "2c10f23b5535ce3672acc9743e3352a0e4ddf48c045bc8ce18cc35a21b9803d9718d23f8cf976ac993f69539e7fae7569162fc024cddd5bdfaff3f6fa5b033d2" },
                { "km", "4e82469b6e5d273ca43f98139a01452e2791e3ab4801e9a9f836238f33584fbf19f5ba161e12d8820c6e6cf0aab68907226076cfe2f0ebddfc4a2af39084df8e" },
                { "kn", "20a8d39e39fd7ae1cc82714bda1b583d9357f2141da5b17b1d1562ca68fe5934a7c85561bd40ede927ae6a29737f434059e7000dd72976bb456d2b54d99790d6" },
                { "ko", "0986d3e4cd171e72e5113626c1e0742d77c47120de0e0b5cf742fb63518e192d5687c7f629739c63c8043b76fd03c3d1a155e67d148610c608afe3128e462f9c" },
                { "lij", "3fc499d0aaabbf1e0ab459685811d9570f4317ecec8d3f207ae72b8cda9649040bd4c835eb8040096504ce13d3ca0db06eacd63879bcfa7227d26bf71561369b" },
                { "lt", "464a44f3e749df1d245f07ea2101461eaa9e206bfd3c2689d21c2d6b20d724b1896ac672b1472d77be0431bd3bae8f71189606c4c92ea03159cd277ed6e98041" },
                { "lv", "668d6107a516d872346f7bc14838fe231fbbf5eb0da46c2a4a2dd9765207357ea0fd719a1481da7133587792ce2e527ba188d513165e6420f8b0e0b41dee4fdf" },
                { "mk", "584ab1a6d6abcb8d39fa451bf9bdcbc20125d050ff6ce10dfdcd16b1b7cfca00567df5f26239131ea9f317f7333622110e3701df1d870bf6512f1663deb04741" },
                { "mr", "6ae0902f85960e71f7871b213b410fb3b8b8a937c4e83c3fafd55d751a684981b6225c3ec3cfe28b800052684180378efcdf907ad2649f0c80d0fd541461b0f0" },
                { "ms", "92cf8a22bf3e6ab1fa31d449d519ef18c0d426973353737f3305e93bfb96ff0f5ae2ca0264a177a181254d3c43bbb31149326fa59dd09e49898cdfacbd1b970c" },
                { "my", "87b53017a2d6af639338ec3405cfbfb8cf5c3dc04ba4304e0d848a05f35f231caaed9f2027fb99d820b24a89f8b777902da28943f0c9e8cbf4a821a86f742060" },
                { "nb-NO", "8a444598a37cfa3c7a589ab6e76477358cc517fbbafb1dd8e588315011de187b69055e22e0f2962e0fa7151475b5e0f9c29b244d981bc6e8476e8aa63a46d442" },
                { "ne-NP", "02ed29e91c76b199a7daf8dc52bb14142c657c57825a7e35932287cf2fac6486504bc7e8b09d4a65d440d65b2e968d3c11b0e2d49ef590a987af79956b8addfb" },
                { "nl", "5cc50fa77ae50b09b6756b10db8cbeebb0a495da889d7aa9f942a9b7f0c4499f5b4090c5d10d17d6fd2bbff03301aadfdaddf469a5019bf684680986618e7616" },
                { "nn-NO", "419ff807142d88b3a1562fed21a615090e5742db0ab548a49ecbbdf7479ba92dec1bf8de12d9b2f35e0ed60abc9e7bbc4fe19239d96aa1e5900e5d4575f4dde6" },
                { "oc", "38c0929d94bbd4b688a75a86439a29c24d76c8ee7e4e857b1aece8227e3c1bdff794478d7882a2edd9e21f6282e4c54147263b8e1ccef112eacfd649c2ec78e1" },
                { "pa-IN", "4079b37b0f43069fc4eaadd850caed90485ab9863ae733638b197a3b9c7fcda8d66300533b9fed1674eddc699fed571ba3295863e27f58e6799137da54de155f" },
                { "pl", "69da35e56c2289eb4fead5c36bd11af086bfb19872cede377299c98497e63679413b6526f2dfa2108ca742375320af5dad978cf476c7c230d62eee2972303f41" },
                { "pt-BR", "59d2d0b91f127b4750c1c62df96433daf21ea39e42e01e697082be44edea72ab8cbf6119864a5463ae56a8df20bf940b5ae23d47c0fa34ce08f1cb4127307852" },
                { "pt-PT", "066e91401bc724ae39bfa050357c64fef16564b9f987884fefbbc5bf970e368874267d599ef4ff3ba9e292d3d0677927b7ce9d813e01844b7831b861ad9cef12" },
                { "rm", "f86784f1a24d06810e433a75a7a35be29d3b2bf3a94d3e5783187c191b1580e7f3c30547cfbfd46c6fc788e5814450a165c48f85e3ff9ca0dfbe775e2f773119" },
                { "ro", "e19f01c6a688d2363e88da47f864906671cd96caac51de3a015940529c765e85fdc4417a8b3207365800df33dfe3f315f579d48e1be80174959596f166c645e5" },
                { "ru", "b2397f3a786217bcc6cdcbf8d7e029cf4d8251af4dcb539a7b6272a071600bf00bdfcf0d1834c3520efa86157bf8cb2d36eaf4995f1f49ef16042e44f37a0dc8" },
                { "sat", "c28181ad1ef19740bfddb68dd44dc66e0bfd716b6071cb4c6eea3d257f4197cfe38e4d1083749a2cfc8ce60d859ef3d290473e69e34670c8706f1a87bbfeb270" },
                { "sc", "02bca9018b0b455af6141bd41a06c7076477045283b8fe9cae104effd6e0074c84285be0b03cd73b5773782f149ddc53499dcad3a7a0cba102f255f184bf329b" },
                { "sco", "2b6eaa75dd15ff18a1d344966ed07110e71b13c407105ba1bd7f1c6966e1c64fbdad26358540cd6b9092527964b71e9f639fdbfeda58676bb555f95856f55fdc" },
                { "si", "66f3aa2a800852cdf04d06f75d98ad4ce44d574ba5e87adf7fee191497e66304674ca8ba96bb14e9124d2d049d1ed92615ab0ea99e7a4d7d85e619cb4e1d1219" },
                { "sk", "243230f6396fdf53eef2661e8cb734dd1066753c2bf3097bf84956f266b624f3268b8e2d1907732f3532a544f0906d9f400b90efb1cf5f2b878807727625dbe5" },
                { "skr", "1e4cef24154564c8f0261a262965d52a2591cca88257d21dfe9c7ac01717830f5f3ebb668668ba1d70af0994ad9a6aaa6ea96000c304262510834524baa14f0f" },
                { "sl", "f13861f6e36f8a78b8fdb6ce30ef22ecd6fcb29ca7796f2af635a96dd55347764b428391ea4046dea56a2500ae02dfea17e5484c6467e0f774b70b04fee078ba" },
                { "son", "47fd5f9e884474b6c823efed3d3537c861cc7ca9d7ba327b5d13cbde0193ae4b9d66eabc31749c3bebb4460e17d51b0d0fcbf83a137b3c652fbb4593c76652ff" },
                { "sq", "481639d12c1670b6937f423e89527c43de509dd30e21b7abbc7b82156cba7e08381dca5d73605df63e64c80e71a9a402ef0d8d93bb0bc8822b14f11f7837b7f6" },
                { "sr", "89d0ba7a83f863fb570b5bfa880ab5157dbbf2baa52180dcc6e2327646895226297f35c3c59ccc620e0bd0b3b2a1448d150e01ed879fa4148cb716967f443cd6" },
                { "sv-SE", "11e23d9054d9fc7b9da6ba152119728f5645b42a346848cc3f164e126d0dffb6d47b15079b9c7d2bedef59f47aaa27265e2201075e175813b324aabac05a8aae" },
                { "szl", "1a163a92e777031b8abb6d9528ac981936c6d7f5e167c410810014d22530bb960a468c22044909412159f9aff28f1f6eb7817be2e268ae568f7cf64282b3c9de" },
                { "ta", "950ca80bf7654c6abbdcdd273cd8700177685c34b5cb1006eba70e9b40a04bb2e4585c2f0364af3e08e05cb49e43ebed9a4ea29b396801d60a2070f3c75405ab" },
                { "te", "eb29f2cf365471e412e379c2adf27771378cbd6549f2b18cf52314f4bda44bf42d313f51b1c97e8577a94ac8c622d1d5548f8e71b23396bbb16a9e4e77ddddd3" },
                { "tg", "7daa05b12faa59cebf4b2c36e90084993fd7d5a77f1e6190ecb92a957df294c5d206890e60b3cf8b43eb245837f23224057c662cd04f40d66c5cd19869ca8dc8" },
                { "th", "92bec67af50fbee8dad3cc23e52c5f614d8e43c9238c2ddeb6c41883b2335b7fcad18797b1abc254ae15c9295de35ce3e919e88f3540aef4501c86a42b81013b" },
                { "tl", "f572cd73d34b67c56b4e75c4a71dba048e042d2c2df04c0989143b7044645053eadca7698818f98ca6ff45f7e7dae1f4466d15b4748506001a7b2d8680deba45" },
                { "tr", "c4e6cde9efb3d867c866e291a4cefc2680332f73af3ed415db88b342602e1761cfff681b0189801377bcb5c9a75308e7d16913c99111cd47a2004be3256aa8ed" },
                { "trs", "b316585e7a9ede756abeded070cd9715734717954da61d4d2c7e3832bad5a9adc91e1b51cec11f04e6039521686b71cb8e01292008d003783dfe35302760fbbf" },
                { "uk", "c124cbfe3de84a8740bd2bf5bbc5a234f1fad0b070aed0a4cb7e246f82b740303750719ef22838819efc5fcfae6582cf088c05589d3fbe47e92260bb22af5797" },
                { "ur", "72314d313ee523a89622f5c7d367f8fc95e409e2893f764929a8df2b7f1f3e7792e0adb8f6aad1114c22ad903182b851f416ee10d2ee5f809af5d9c006dfbced" },
                { "uz", "4599fc478286d9ad752898cc870238479748963bc7f41bb93c6fbd59e7ca079dca28f5fbfc7c54f2871987bfec049d4e63e453b60501d4f160e12fbf3c4be4cf" },
                { "vi", "3a7dd79f7d8e3f320d5b5bba6f6b8ef77db4fd01964db88db826e20d0edef0aef36c5a246ac6f6d4cad18b9c387c91953b62fc4ef4c840043ddc55a95cdb52eb" },
                { "xh", "d7ae43793be75b76e1a9ac2d5836fd2d50ae1eb136325d02477dc4265c0c7ce4df2f393770b79b6aa63cdb436182d142a1476f71fd539132522411ccd627732f" },
                { "zh-CN", "c4d50c5d1b42798e1649dab0e437feadfc8c976397c2005bfd599a3530aceb4c8e62982c6cf17b44d34b0a9b3a9c338ce9a2a2dacb80b7c69e923ed4822de2f2" },
                { "zh-TW", "99b5ac28f00ae5c0479c4673a01d0219fee7d8c21d41ec291bc05d733d785da4e82115b07605a281d49914ad979939a7ef91ec873bdbdafe9d230a09c39dd474" }
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
