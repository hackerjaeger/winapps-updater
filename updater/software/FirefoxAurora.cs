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
        private const string currentVersion = "100.0b6";

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
            // https://ftp.mozilla.org/pub/devedition/releases/100.0b6/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "a4f415b3560e898458ca6c59a83ca6b8cebbe7cc2141cb33c7479dcb4abc1acfcfccead08295e9ce55282626e5cd82d2e8c518888d12d450808e368b5d494d5d" },
                { "af", "155a680d5240745506103ddd8c773b935d5e612aa3bf3bda8be8b2dc5ef1b760acd46ad94a61dfed281bc2416693b78703ef4dce61598c05249b7a1326884dd6" },
                { "an", "e338fd0d68cdadd1c93601581ef36cb289977b6b7e46b9bbab33b395c419fff340ee10c99cadeb2543796bb7d090462502461d4fa027c471cdba4bfbdf40767f" },
                { "ar", "3e871ad6f8ce5ddd0aada861c32c0a3c8b77f1e2a5a6602f6bd705508bdbaccd1d65b1c724904eca4808ad75a05cd618100260b63351e7a14bfc53c13a85fb5d" },
                { "ast", "546fb3b0056e993438f81dcf9472c0f1f9049ba5ddea4d845d31562824e52f5bc0d192fede82e6104a447dfe17348193f42555be6210e641f3cf8034ba3b0ebc" },
                { "az", "087b0cc0cec213c499b0a15429f94c280fa431621c65c391839ffd211c2bad42f51994d3727323345efd9eebc76aa0f5034794ebf69e3dab7ac03d4590100c63" },
                { "be", "f14c395b622b2d0634de3178d3a0e2bfd29d3c4ba567627baf819ba23875c3898fd61472fcb4c95eb7e5561dae25f021f31e929016c374b1c9322af06b2eb07e" },
                { "bg", "4f6f24e0a8d8a5a5b97b003cc693c1265fad363e6c5e6f144e9bb7741d10a079b8740a6492b6d50587e3eb46f7579c7294ecf86bb92249ef14a1f0a95407490d" },
                { "bn", "73bc6badd4a262e1393bda608580ac7a1f572d8f4ed53e05755585b09b9aa71baba872cf600571027132cf3bddd4806f653b8934f62af0ebd56c9d029e9d5745" },
                { "br", "c1d5ab05018db1d72cbec44c7d781a8b5be2df1740194133f5fff4b2011e4cf16ae8ad8dc08cd06641b2d6f84ad35ee3e7e509832943150deb3b1b652d55dfcd" },
                { "bs", "9dd547027efe43c1ae0d05d00e298a9d02092ac7c797d9ceee433e0d08dea90f8bae27dba210cdfcd3d407240e96a1abe4d66229074cb1606e112f638343e4ed" },
                { "ca", "d7b79dd5de6a9a10b3d92a11fa391c6a46beeab0c8c684f182fa645a2b5cac36701613f647f46424bd517fcaefebe074715dad8118091799adeb9d70f4f3151c" },
                { "cak", "ccbc2d58be22b84b47204f66fab0c94426e65fdd0bf4fffd343ec20982297abdeee1545fd6df07ce11f8d13c6ee702108ed4e711985d6f8d04d98fd237f1f844" },
                { "cs", "281579ce946eeadea96230382a1bd7f4c9a5835edeaa8161d2e44abcf19ec534deda58b623be1b08412cb2154a0fc5e54d8a6a0a47f0d9a5698c1bb51afa41f7" },
                { "cy", "28e1f9dac54379bc9ac10a02c0b8bc4dc6f16b7610a4351a0ada556c6997f471d6bd3942e1c3933e125f2ffbfc3b1aff366b270df4560b5e8799ea6b834272a0" },
                { "da", "bde111377ea7cf7ac735c95f60ebbde875c39c6f91b01dda09533a508217f981ed66479d8ef92152bcfc2dbbb0571a02581cab58c95ba38c0ee9880d493deab5" },
                { "de", "0edfd41d6c71ce99f815d93e2e01f7a3e1bbea67e1c1b0b0461e0fc005611c5d15d0f121c7b2d26025485f42059c7455f3e6fbf7eb7244828423474991e84a5b" },
                { "dsb", "74ab1742d47aadcd3cbc1c95460e148ef5c74bf4ffce8050c1f6b17ed380808e2172ae47288e4c6f28f30ee60a4c2fbfc7ae485b2145ad6c100cda0a2df21efd" },
                { "el", "388fba17ef2ae97afd6fc2c8d85e882441068c1b79824f86083f711bbf98105d38d9ca149ffb4c623f92795e3b08d4a13837a4425a7bc7151d24545aa186186f" },
                { "en-CA", "1f2f0ec27de876dbcb4982100c1b6395de7b6209539da1dcf8a9afd405d5638eaaa689c65b7713770ad350f24da5de89dddc59572a643c7b066a9178020d28bf" },
                { "en-GB", "ae2475d7e19aa750d2da981e49f9cf6a3dff0dbbc230b9f2b3537a375e9a5dcad7f5b449164a5eacbcbe9c7f924d50ce6afd267114467e6be3426c6fadb8d563" },
                { "en-US", "7026bd221e90d001608c46ff51c082c6afbe06c9c687253a5ead21a4fb5d9097a7471b60c568b3909172590717e4f7e1679ac8b5b0d54d45f74c1689f29c9f1d" },
                { "eo", "19cdf497767eb07cfae19f83c6c08c10f925fc4bc2a89b6e8ad546765a877baa150b48c91e8e048f792bc10dca93b64df3884e2b634acf7a2786c782d8ea9e11" },
                { "es-AR", "70cedf8c0bb65b93913fd2b7e4eac2879feaced167e10b3d0937e2f5567202003aa61880c810ac887b64f18133aaf61bdf1b0454107bf49aa2f51f83ad597798" },
                { "es-CL", "911ae8380dea6c63824e3704f9e7ecd0259867e3b3b7d8af6309d149df008f0fefe15a08c5ac2139761001461f5ed4e7115d275c16acfdbb97e1c00c2f964242" },
                { "es-ES", "08f2a07f0c94e70bf1b65d203f420814842c167607c884b1c0173a1184f80caddd596540dcca35a28b1a0d1011133b5ece197f175e8a601ca8e6c7ea0e925db0" },
                { "es-MX", "8c078cc55f71fbbf587d066d46e3cc75e64b7ee3e6c1def24e830360af629e7d93d7d710f00438b8a7eea4fa8333022cbdef2c40d2295ec1f8d8d396124ea497" },
                { "et", "af2bc24190a9d882037beac2eba9604236e7d5e9063e7e9555016c9e4618f373059d415748ece4c55bd49b1acfa258b9bc3eea707f4a4d8622cc048bd5c068cd" },
                { "eu", "41d790abbd532d33ef6e4c79ccc1a55513834daedcf41acfdeb7cbc9154525d1786b10c1f8fdcfd32c8bdff83f137041950fb751323961cc6f29133e13363e63" },
                { "fa", "d1c51f47b1de1967aada20ff20438d6c1bbb01a064cb5d164d5b9561359e0b6345563ddf995217f1bfbcd02fdefeb4e110a671560409598723a354e8ceddcfd3" },
                { "ff", "23100f23eade3480d98f8001d9c84547552b20a135ad340d979d90e36bca19852668ca5fcda6c8b787b86540433deed6cc7e682ac42a36d3ea28212db6ab0fb7" },
                { "fi", "6e0dfd1e99c982498dbd0cca7f0cb1a21b72dd6a8174e587b67ce2c8f0fe5ce2836af25d601abbffe15265454270e72a54234d0413bc6d5afb58d0b8da2cc41d" },
                { "fr", "bf9382164dd9ca54febce31504811b91d0cad2a5c34f5fc8fd3be7f5c34557da3bdb566e4b62b10ef2c2cd74b11c03b08c27d8f90669b8f7917064f939617a80" },
                { "fy-NL", "c750b1cdab54f43209a55de9f93cf2817427c6979046878cd636088e9cf0c735ef1c66edfd8eb5ae944f51def0777389f8c073be3ef3fb28cc0896bee06e5a44" },
                { "ga-IE", "b654e326fc43638b3de09e22414a26745d9da5f9848900a7033ddfe818558be8d4052b21a08fb26ab94d35efaec3321c2a9e7e0828fe817526ce7114499b98ef" },
                { "gd", "4370d0499e4d5c415f83cc08f69775555c137bac04e04622793770a37c63278015cc952a091d4c2978f36c4cbad6cf66c60c2a30becd53bf7a460ab9712d6266" },
                { "gl", "de595d1801eba8f2f6769cd3a45b799c8b9d70203bfd433630632029356df4374673a72a1022a8abc7ab65dc53c0be15834b400f251fc260d19ced82ab1fbee5" },
                { "gn", "2132aa24e2c762014659fb5d66a28377e790293f4ca0b9d201d2133cef33e16a1eae908d5a6c67525ae9df4ae05a119c393851ecda3a65975ad16bc68129c540" },
                { "gu-IN", "f396a475849146cdc34bf67fa9e3c3d3d654c1b802ee4c0ecaeaf8ec8a95995ffbf87696238ee36493915d23a3b4ad418af242fff0b7ec3d23efc5eaa04dc70e" },
                { "he", "3576593298622008156fad69bca64f538f77fd0b299ec2afacf8be31bf48406da5294f64ffd98ca5acb2fcca01996e1815e82db33663a586ff15c4c45d671e02" },
                { "hi-IN", "16a72593b0bb9b1c09b63b75befcee3f2682bedadeb25c053a253fb55f7c1ffd558da460a12d09810191539695f5c299bfb27483f2041f3f11b94826dff4b324" },
                { "hr", "565daa4a399f141e582ea87d611d51d70e00f53c88a808850365b9dbf85f839c396e86a8b39fd07695dc4715939314eec6e44dba1c667ebb2d78d2774072883b" },
                { "hsb", "6b444bbf3b6dbad269419ecab171d6552ede4cc42a8c06e894519f4fa9c1b64b9f95381228f1f4b57d271bc6955cef9445aa6de312a548f90c3941ec4566d265" },
                { "hu", "f35b4d42a93274b413c010a635046302d6a07af6f92d8f253ac448c56d4e272e8108b39d4e28b4b37200d17cf37373fb7d908037fd022a511d56c6beb6716070" },
                { "hy-AM", "68a84f1acce6a548fa688daa76a89e976fe9fa627f678012b414d62d90b3bbf22b2c8526f588206f8534ea3fbdde137c794a090cff027281e705bb989fdd7e2f" },
                { "ia", "422994dce73c48457e83927230239828bfe0cea799d6a534c5a72e8e10dfe3caa4ab3c398d15121d7099a4cd7db3de0ee6005bbe3d6b3dd21fe305835330f956" },
                { "id", "b891870f8abcb44cd620db6fd47f601842fca09289176a986e410edb56da0557dcd33d7497ab4e1d830f2b3aaddc818e46d572d7df6a54d127496e701d03dedd" },
                { "is", "3edd3f75d2899d9d76b55d5c610f0588edf37463f8bca48495f48e2a8e98504947e89e6bcc85401cc1beeb8cf5822b3bdc2ef226c85665325beec6e9fbfb49b2" },
                { "it", "18654f3cee4b71484a96a4ae440ebc9db46a56ccc504a6de6dfe29947395c67e5872fc803cc64c175c8e19574c175fa47e079c5b19b2257c09cdc16655fae082" },
                { "ja", "e99989995449fd73b21f106e91363bacf8828a5d2ac4c30b4213f47d4392031ff7eaa0a86a42bef8994ef4c90a6cc180ad163cb65c710e3c1b4d69edf5d2f00a" },
                { "ka", "5e51e71e625a3b75de50cb534c7dc0bb49adcfc8b73622c12ed434c6f727d5e564de967afd7469332a89bb2c6ee0662bb9b1af0814c342bc06ef74bb46e44f59" },
                { "kab", "58afde3f93f904cffcf7a2a7c5f7c554e40513231a8006b29120daf6888f25f0af497b4629b9d8b169ddcb3b7bd0fa3da0b7a57f4e5bbad1077720cf95568221" },
                { "kk", "c993901a0d09a6fdc1dd9670fa1f554161f4edd739ff8a2c917f888612872c34be01b9985c4213d702c389fcafbec41e3bd56bfb7051115c545ea8d1785c9764" },
                { "km", "c2b457c8ed98da6ff7f4ab0fa85a1784e86ddd8daede120cc48c90816cc70720c91c8e4dd52dbbc861cabf2a042a0dc5f4273a9f82b3c174b32fbd15caaf42b3" },
                { "kn", "49dd3f65f36f3e9aa10cad08dc625f291f82aa3751eaf28d7e733bde8d2f864a3ef4310236663d43b15024075cce735bd97445c687c3a3ef84421f59fff0753b" },
                { "ko", "761ccde847cf4729f8468de5cc95c8818bb272150aa4054c8ce47782ab224616a1753a843c57523db9dd3e3dbc642ccd8c342c2ed7b28a08bd614c9f9a48b2f3" },
                { "lij", "57f7eb65aba37fdb76bbdb4511e7293e85d95ea587045af579845e72a3bf21ba5b6bd5868174f8d2bd774d24e29c9c87d360157904688306aead78b9f027d2d9" },
                { "lt", "a4a187b0e6964fc75d1a0ce62ca3d9298327d6512a623425d6c22843ee63d50e186046e4113587c9558b469337c5121895da12c4574c6014d98bae0c7d02777d" },
                { "lv", "edc71c945b76f66a8c91f895ece50e3602a8ac76ba8bdf533d8d7eec42ca32329c95cb3579697a3d4e93d3ffe95b8ada96caddd1ad0e49e2e9ac3fbc95b611d6" },
                { "mk", "5f9cece40f3213784f103875aa529a0c36b8a2c6a62ac6bc0a63632903fe66420007c64baf4c5007d436bfd7cda9455b495fb2bcfaa965fc2adff9ee29035c20" },
                { "mr", "186b0de36f6fb5c918853b807df7539b45b9ab3c925a5e1d1c2c54a7bf6b40d355b118ec4cefa6845803a471ba82a87667a228828872e2b3562d4d0ebc45cdb0" },
                { "ms", "7825731c3063d3ba46c22a24fefe045177a7590d77c2250fc8724626c9492fff75a1af5b109f47be4f1dc5943be9bdc2ed87905548c2b89a1c83468592c1c65b" },
                { "my", "9e65b7bbb55c709a7e374d43464ea4ea11b3fdd1a25980cee7e903bd4bc982f0cc13ba626b6dd06e36b3af56d1b90080edbe08d2b4d8174d34b4f2d3dab2d887" },
                { "nb-NO", "da2e377b508babda36b4bb178b254c9fde448bc36c06fdabea41e3921b41c2ae2f92ea54824baf25ff8254cadfbc8cf4892df3bb83633bce540b6dfd9d19a5e4" },
                { "ne-NP", "91150337813aeb75d71e69f8e850184e8b49b8ca1ed2f06d7032cef8286c73e8cbc34348c704287b1d2f7f9fb1a0b064ba255ca2397250cffedf65263057114b" },
                { "nl", "0c383d321aec8be05048ad774ccae6ab264c42315ab5bb44722e14e1d8da5e8449db5a7e507b5727f2d51583a7e850acf07d41149d02bb26f6b1ffdba7d264a7" },
                { "nn-NO", "f91a515c8e0826d269705ac93a66bb028a68bd30c5c86ebcb5b1dbce3bb4d9f4685be4c28cbfda46853c42d981f03b0bd75b7ffbc5c4a0f110aa8bf7f368df4e" },
                { "oc", "581bd727e07e94261d17bece705e99e7b3dd5497914b8eed1e3c29d9f88be39cfc1ac26e96c66d849c134ce4e8e55e53cff220cb1903434cc43fb24ea2d346ad" },
                { "pa-IN", "dd35d795baac0fad12214b761d9e69011f756f1d544c10e199e5aba4ead4a8d88879b30f0f15ffc7a22f4124e4984ab1c70e97747c2ecfc2a88af592c56da142" },
                { "pl", "91965f342dc6bf232c050ff3decb2e3f8ceaad02d16b76877fe6563a297df2652c0dcea2a7c775a89fee75d0f6d9f7708e7341a23ed5362a283bc15b77e5c893" },
                { "pt-BR", "76d30981df5b0bbf9dfeb64c279bf8e20387e2ea4053b5fd71be8b9ed0a3b6fed88fe03f05c422105b03487517737ebc9d5c779cb9592cbb82b3e5b24693161c" },
                { "pt-PT", "cc59850740bbf37f6600ada8e24287882470af329267650ed9da7caaae27f87dfdebdbd017131e2d325ecd60db1751585b23f9b28aedd67a84374e0296fa8ee2" },
                { "rm", "4c51a2f04d35133711325f411d8898f3f9c981372d4d31f195727e79a437b14305198266d0b309a925c221205ac372599d78c98c03aa19058ead0d64fd7b867d" },
                { "ro", "4feb8436d3adda0bf54cf2aacfc50f6656af5f1d9c7a760422fc4d447278f8789fc8948c2187867f1dd5e9a9089cc981061817d0462f5160a5115298a762595c" },
                { "ru", "50e8b03c57875d81c15504cce5cf020b58de8ad7c87cfc2c8548e648377cf74ff514d7de33f8d09d4ff5c7c98cbe50beca7cf4f55c82b3421092733e423022db" },
                { "sco", "8982d0617288cb79c3320b7cb53af15c04c9e79deff30238bdc944253c3657e55e6863b5f8623ed41435d19d3c8528b52906223601b67640d3f83802917b21ea" },
                { "si", "e280c16c43e6e03937c219026d9eebefa9380a13c3ab582a31178e6663984e6840013fbfa91d7195221c2564f1816eb926948ff5f1507879a78088078c2190ee" },
                { "sk", "fe4eac5d639ed798b842ad78672e3620527e2d013fb9704b2fecc9bff89a0fe0f81f2d6d1ed0e345e1bfe4fdddf39d29f8358b41a46f5eedd6ff116dd3028609" },
                { "sl", "0be7e0857e5121bac1f2ac049227e5b8db0f34751d72929dd72ff0eaaa36500208ea5881f2b9403763e16c035c5e1e8a62674ff4a92e5c7c8414d75327a09d0d" },
                { "son", "acf4eb5c093915d077712dca6db5b67ced190641ae50cc676432a7e5f3e9ce7602826cfd2a88b422e5df4b30f79a435710bb3ab35b931fafe04a3650b7efa617" },
                { "sq", "5d1316a7951ab03861a3b8b43297900eaea6d86a3302628cbae6cef88452659e5a597fb71b69e699f8f64db6270ec998764e0d8f9e51df98704ae010c3843860" },
                { "sr", "e63758e14579055b28b4c9140d502af7cfc4e5b6d2d74fe62a5850d342de1beeffcbb70204dc517a1a241c01a3af8ab6873f300490c6e458860515760f2d36c5" },
                { "sv-SE", "a9cdcc8166f2287fd5017ab8a8ea7cc0dd3d4c5cc3eb6e0a109433b7e969e83f78143b2e7937bf9073c8ecf3c52db82247b1ea7c6201d64305aa3781a3cff5e6" },
                { "szl", "1f6a109ecc0caa7b25db386e761f21b7f2a6b73cd27f8d157eb6c0f5ca14258d20bfb9e7380283ab106e970557b05b29269f17411597b0c6857a33452f865936" },
                { "ta", "b2d9d36371b93f3254081f321c07f43de4fd216f9a37e9b81a26fb9abb945ec450567dd488daefcbb08db568ccad4d3cb9a16d5b220772a9e57a03a18b831382" },
                { "te", "c4a1ac5769b805102626e9acbead0cc569efd09d0b799a0030d9e56481ee9fd35bfc677a03df7bb6dfb82f2b575c6dad1b11f1a6e12e5210d8c06fec581ae556" },
                { "th", "dc1f3125d3b34504f473b06ddc5e248ba778947dc178ea3f6ce3c23f864b1c29bc2e6eea59a4852cc2f1452558beac977877fb08afc86ec37758b6bf20735b90" },
                { "tl", "5ba7445276ef208140e60f4041b64e8d21d1ffa993c991be570fcf6dadd7b4b5297e52313af5c4ed941b94a2165144d5d87a2bf7838a3059772330f61f924fd2" },
                { "tr", "8198713dac09c4787e382897fdf0b9410e1117d413d23c519e70ae9f802077b5f0fc3db859f7240193e69dc20b7b04d808fc9f41355b2071edbf21007e39d86e" },
                { "trs", "43ef9f12f4f0e72a889b25bbbbacf360c4430e74b09712820c5b2c95918e3352a86e7e0be3e167375877a7b9de1c056f42781b7b3893910f1b8ef60838b2e961" },
                { "uk", "8eb72c8bc197bfe947ed3c5330523e19bcbae0d37d790d9bc82176351b23fa964248b3e56e02cc1ab262ca4d797c0418a3a521fe25b6dbed6250b1c4bb85b470" },
                { "ur", "d9e3e788bb795b52b9934915bf64eab5eefa94ac15e70516524bb7f181b176d8f35fe50e1fb1ba7c986a407ec70b29d9c3be8df59a2fdb9e9aca1499cea5c700" },
                { "uz", "06392a3856dc9a161fa7029e04a6dd21ece6783bc1772912b3f9612e64f810a1674b7edd6b79b40d863ff2d1ef55e1ed7038bac008afd7cd46cbb64b733f266d" },
                { "vi", "c8b0d0135145628179592c29d3e39541a18394afb69329053b749232b9ad86ac68aedbf247e219b47bab163a3fe6deba3567efc2e7b15823097590b979785fcc" },
                { "xh", "a1c15fb81bfa10ec7482ba98a3e35071de4a5de6efbc1f5064feb943cf17f20781ae63577e225d9bea3daa1edb96805a3dd3d8448a20d9b268c20460d1e09354" },
                { "zh-CN", "a0da94ae390c27c29dea9a64aab20dcee8bffb4c3c379061241a529df89c738894ba35c5924272cad7e491b5f61d2773452910b09ebbf4802429545df803a02d" },
                { "zh-TW", "9ea188d12c37691c0c5fca9b992333e589c5315fb72d64b8586b801f08c5585c5fbe9b084c0cda032ad2bdaa174dc29bdc4eff0bedc6a3e3233a914aa9576435" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/100.0b6/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "ea4dbf59f1676bdff876ea38662db13cf811a92882f564678f27cfdb2a7846f64a8674f8ad972b0c0edab84eb1a14785dd65ee4a0e722a03fc2d96aef1b1032a" },
                { "af", "9a7bd7ad9c4abf0647f9a15374399d47b56b6f39696b0f047424ac6d3f5f07cf898f5c9138b52de02ccc99cb225406ff2ff761af008d3975b3d47f60a7a5c4a0" },
                { "an", "c316b8718be0fa6a802edbaed40202a84b6c317ca2b9b7b2b20bdb923bb639b51a5ab8e1ad1f5735ec06627c6842e25d2d9b3a415a13c6b242fb6e6902364100" },
                { "ar", "f4718a32ad0e426111ab72b0a426d8671006eb4433fe74140095b7ae4bd60a396cc7e0d7825af4c567682586822a91265fc01a5093d1b7265a3f2ed663bbb50e" },
                { "ast", "54cd85dc86ef0a8adfb9481a0c9abf7a40ef768c11d3159bc62e6e1a9eb91165a443c0a8fc920bcbaa421b30ef14f996c9f6c46f4a8e65b7c793e684c0b92b68" },
                { "az", "e22d3125ee17f8f15d3a40cf2c8cab17937a2507ab17562ab6fa15a79f94d12cf1b869d60065efe68c4532227f7c6b18251d222e73a8a32f5b12ff386470d8ab" },
                { "be", "6bd382fc872326064deb523346409f18656cd24addf48f815846d1c274105d8dead7350b0f3852f1c9ef1db992821345ba887c9f9cca288d46050183cfab9ea6" },
                { "bg", "eb0af7c7b62bb22bc88e749cf8731c3305f5e3d177f690322f64bb368374e4e2e7f9d4c41e6c885e14c11cf4a4e7c27c47b802d17bb14382bd6af2477f1b13e7" },
                { "bn", "e37a3f66e2d8b23acdd4d1387e62bed51263e9c87847abb32eff59e5bba9a1144841dd8eee6b2db18ced21a08930948fab9aa35b80b62b62519a55d412034a80" },
                { "br", "37ad5d8405f1095c68e2e2aa5bdfb5358a842fd961e6433c235eafaff08f14de92afdef43caba3657290da3a3806897c9a3550737da9d0b70ec865c7fc8b6999" },
                { "bs", "33a77e1d25d4f895cbf937c79412b4077b16e8b3b11121e9e145e2f5e106d1713d8dd41ce19f98f7a085011f5ccf695d78b330f41baec10b403a1c7451f48999" },
                { "ca", "b2d0b14b68fd444ab055d44c73feb73a930fc7307dc73e34a156bdec01fa012ea94554c27a80f3416494e3329af8810675fb120385959d4b4b94aa29f34f6931" },
                { "cak", "2ad16c6a1853ca2aee3669746256b1389a01462ab3d264971c0e84ba87db0d2360ad985912ee7ec69a95df3f2c9beeddf551fa766b116db9f87c063becaf0ba4" },
                { "cs", "f2101962b8c64a618efd070082f88a88330dcf222749b3ebeabc7123f790b1899862945bcae7ad7f46056df947e4310a21aa6feb064bc657eb6e131413458dda" },
                { "cy", "bd9d78f13d04dbf7ce6641e32a3de6a91b186fc51586795632ccfe120dedba066907b0ec24dbc3af804fa745f097a33f7ae47ed69fe03342379a03d27010b09a" },
                { "da", "d60c08ee5059b0a45b8afcd09694d8f274d72ea348f4172490649d1b6440682b7f39b601a6ff77650742d57a0103e0b7330716b3b155a64f17b331f6c8dd3217" },
                { "de", "1f28405498a0b4b4f13d2d506025b3308102bb6c85ff03e5234d981b437f64eaa7ed1f447e680dbc704a6477e23850d4dbbb6b40ce5eaeafb7e7e0614663b643" },
                { "dsb", "75c4bd1fd3c8f709c0a5c1659e84dc29fcc6c8c75afb47f370e78ef2c22be335c1b791ea49a5246f0df2066a580aa75b162107824c96525e6ce752e1882e05e0" },
                { "el", "14f277d65e9b126070cb52d589091ca828682f7d18033f174429ef1670a9f786e2f4e4bf16439e0c196cbbfa5d72de0b2458d2b96bb2f7b20b6108ec51939f5a" },
                { "en-CA", "cadfa0e6e2b415bdcd6f485f98b3f772223de93669e6b1fe7971406dec26cf8e0ba1943f28930042157556a3e099c74354f5c346ed28648ba8f83e72cab9aad5" },
                { "en-GB", "c878ce0a60b7685e1491eb37628c4b1caecaff17591842d5f5a39938105f20f034b70ded0942201c63085f4ef52adf8005ab247c9cd40762b2521a08ce1a8c91" },
                { "en-US", "7ce81d584287f6b3141becd9dd88fadb17ecca77123f632fe23d0507aa46c6a284977e56da290063c9fd53c2193dbab50a7be5537b5f67fc12894901f4e8c944" },
                { "eo", "3cdce8ad507bacbb4875f6f0cc8f74f0e9f61496af0a1dd3e8d33bdafef7370df541cbe3337190243d0fb50f626815c7c14ef0c730532025915db60d02361820" },
                { "es-AR", "adfe07a2a3bc8d7d9454b3ccc456ccd84ef24c93757af05b101717cae9f44676367d308afaf19b034e86c3290ed8328a40d2aa24a887bf71e25acb4029b67ed4" },
                { "es-CL", "24a1994877716d5060b76f25dfcbb3f2fca1b0dd13d7f8e122bf1f1943a1267446e333fdb89a13fce08bf6c83c45d788eebfae6bae79f011298ee28e4faf58bd" },
                { "es-ES", "c26849e3d31b99058c3f0d9ba0901a7b401abf831b5c7eb84340a3f34d77cc80df9b333ab910bc2cb61c1b43048936dcee888d220b867b99fb2c36f224cd705d" },
                { "es-MX", "a949961e23c7a3176bc2cf5b753b767d54d50a6a674e775d0162d61a1a6d278882126edbaa26d36a61ff1acce43aeda3899c9739dcdd3047e0a1c5b59b042607" },
                { "et", "0924c263a4285b758051deebf9932b69b24b0a801a3b334820c13bc19aa039d2aac90bd51a9bbd366e5cdd97010161f603d5f3b7604f2117afc5acf7a17f531a" },
                { "eu", "63f603b9f3da3b9b833c184986327b8e8b23f3a01c41b41446776c8eceaa0d6df3b5637049f74e45a08fcf626ccc8b2847aed13c2fbfa70e2bc6c7646fe991ab" },
                { "fa", "29b7a225e24419f4d898ab0357ec97342a4e37a11dd116f194d3581811bd4b475cdf81151421dbb007a39483ab8ad9e7499c018336beb00425465f6449f096ac" },
                { "ff", "b2aa776152acd3d65571dfc81d96c6acfb95669b830c88fa8c0c6e8868250fbc382cfceeca308b06dacec8f8ddc87b399511de4d00dccad8bfa26f2bc22aa4e1" },
                { "fi", "9a1bbb80f2eb391a87dfc48d096991aed23176e73ef5bc01282d963305ec15023df0dc5e69be63513fdc6f5307fef0e68b477f345fc76bc95981068e73f43b40" },
                { "fr", "7e4c6842d3150986773ae870f4bdbed897c349e766579c97ae01fb6021e800e5d3ffbdae2ed030fd111916831cd22cb74f4623d5ad229bd308aa2632008286c0" },
                { "fy-NL", "a0d37669c4847f73802e8fa48a1fdb170f048d335decc28e37fc24a4c7566adaac32479b08792edf19d5904560344cf6b1de9517b145fe9aec0436b21f28563f" },
                { "ga-IE", "0ad439a4810b0986b599ab2adda3bcfc55798007ed5fedeb8518b7a4ef62b9c0e17434ac56ea4e488fd66a23b915a7c836a9850b34d8cd68defa27ac5bd8b5b8" },
                { "gd", "80df2dc9abfdc3799f1f542b4bab974f3e6b0c35957c669a7a2c85b57e8fd172c6737ecae52d95013a3d341334f896dab36f5c6550be26d73552dcff412aa2cc" },
                { "gl", "bc0a05c727bec41d31151fe7a95e3cb68303f41954ccd9b64a7e6f0d9d766bc16d0d5d7ce9efe08d254fc0fdeb17b9665f5c94ee09668f0c0ca63e5ab5b63f3d" },
                { "gn", "92ade1d21c87ffebf7440042d18c02757be2ba905db274b12795d2b9103345ee6aed89833f2cf1b4ab38bdb3d19a890f7fa95c9ff900b7c01bdd97f90b40e12d" },
                { "gu-IN", "f3643fbfd5c0f5cfe44646b6722c1fbd37772bc1cf7ff31c551667320289bc428affcf0ebb4f8aa8f4d5bb637520a3154941c4e651876bdbd86adbe3678872e3" },
                { "he", "87cc47d2cdac4418156b0af8a55f95ea7e70406bb68f3d5858e24393a708b23d9386fffca2801bd3581aee3cddde82d867119db05559b01be8ef6cb77abe785c" },
                { "hi-IN", "ecf85a3de6f4227ab088402e24911500de65da3d61ed7194894eadada8b48fb7094074c4b4f977f7b317f65526337a9d41f9c4f9ce192841a831f6af231c39f7" },
                { "hr", "eeb8f997b0b8525c62511f57f937927fe4db0be318ed89eeff641c193cedb55554660828469a20df4ecef070b039338cbd7e82222d8f63dc49ac67d9608fad83" },
                { "hsb", "8d8c1c636d5592e8bb7bea95a7a8452e8a6462c9d9c7df11acf5ebca441be84cf5674c9dafffad3be3532b4327050d1ba21e332367ca560b184c81de4bafb542" },
                { "hu", "a77e9f179493ed3f9dc0e3b1024beb9cdb13ec668655b785abe5dd905bea5954cb26a9225d9794ddafffe836d5eaea3ffeeabb79fb373cc3a91f876e533e26d0" },
                { "hy-AM", "ee9cea428ec1707ccf94bd956cface47eb5420e9f80cad9aeb5c9195cd174d969f9081fa16ee791364d68b4642b97861ae14e0b017086529cf607e3887a400cd" },
                { "ia", "6302a426eab4456d96200ebbae128e00e6fbf6a9b443d80deeb9eb6f0975f55b1dd81fc80c31b7d9c57bc413ddb626a17e0a15faf59beb84fc23ce8b76f1d8e5" },
                { "id", "32b40a00fcf871c51290d326ea97d8f4229facd91df27308de3545f384806063fbb02b1a4d2342a6e34c7b76795012a7d2576f3bee00ee5bf864865f7fe86379" },
                { "is", "6ea2fe43190f0c1c931e7db1159a0d001f5b38601b32fceb418c81427ff4fd0b2e8f6ef53bf3261a4ab25ff29f15d9d07d5fdf66afff6e82c4e7345f1344ad0c" },
                { "it", "083dd79409f6088af312f3bca19f46ee43705a152ce138dab191665cd77ce7346452f7372e24d454fbf2a399a0cafd809fa0891492407d6513aa54931c91ed02" },
                { "ja", "85e699d569613044ac5d592884dd8b23f07a38af5fcc9ac7ee9cffdbe466852aee4dd146f9b0559cc94cf71c4a538ece99c60fa1018f4c002fea74ace78ad25b" },
                { "ka", "cc837a5cbdeaaecfc909267f5e0d69e00b57c589d04cacaff90f16a9f8198cafb8e4e11cc947fe6c40f5bb251d030ea3143e219756e28941ad644be12bb1db4d" },
                { "kab", "87ae5cd60e66f285749e6ad45240652ba3b05fcfd0fb1495e412dd0063eef5bb71144e707e430b21a16723bf3d6ab91896e67e0c171029c7f86d39aa1d9a2619" },
                { "kk", "5ec339bf8b814a5e91c6610dfeae40ab57f0b1c52ab2e21480d088ffe6b03cfe10265e676f2c2bb97a7f653a73e6761e872965195c50b7d8e94c167135218342" },
                { "km", "1e6742fb6e5006d4696b44c4be6576422a34871f73f93e141186a5576e0b75671e5c662d882e8610d74db4c7d6c07cc00aa15733b3e1b61c56cd07db9030b752" },
                { "kn", "b3130c39fe7dd8d01d47aaa74ac3173f737da3c1dd8beb2391aa3ea4c77a8190a174aa2ca7d1ebba47e4f0335c6c9b25e7e168a658ef32b186fb0cd5d9412bbb" },
                { "ko", "f17deefbebd9cbc816104d62cd91abda665f3fe4c625c08a364406d35040c49ddd99c41b2aabd6c70c39884de66b7c0156aa041624d4c80298c52aedb00a30b3" },
                { "lij", "9d931c2e970018bed4570fb2f0d7a3dee113e33a30a49d12a1660ebb915d6dd10632fadb13b8e9a27379b328518d67992c963fd1635422b205d85d9d44e84ef4" },
                { "lt", "51313c1188c60215a0ecd20499811173d6a31074c3b4d651264bec7b1af0e4724e40a4a863fdc55fb87516efb211cc356af0f12fb03b4a59efa24015f317add5" },
                { "lv", "0e092e6ad6df66cf22eb1e172468312886ba321a346e415479860c91c32223a32d645932c4304f46caed55a3213f796f3ad26417c01b37e8e977131506c8e436" },
                { "mk", "27721362bfa6402661c5b5687c2ccd13980fc577d649dadc4dfd769529cd89fb16096b9810feeb37fc9fcbf228bc76d8dcbad34752c24804d8158247506fa5a5" },
                { "mr", "5f3cbd38b0e6196287259a1174670d7eaec9052cf03a9ddcfc9c2300605556256079d6aee8d37823d6452ec2dace24ed4a0538942d3f72bb7a47a5f61e50e0b8" },
                { "ms", "9b4e6714d8cc35ff6b9a2649e04209d0ccaacea379a78885af3830fa7dcbb18b8cab895c87938c89600c16e4f7ee7b9b667729f03fe5276147d35272b0b03e92" },
                { "my", "723b0d2d75f25e90de1ca161db7a9db07c8492260d167060fb3206e9483e8d8b5ace4e7c28d1509c873c482d26144121d64c617390959eebb67dd1c9dd7811b4" },
                { "nb-NO", "10972be43b9fd3fc54e0427c9aff32aea589b50f91741268817cfdd5760ac9711249979b95f38ce835b8ce8152861bf64edd406427a111a6dae6ade8099f1f7d" },
                { "ne-NP", "29dae6dd3db8b3f7e7d642802c055830537b3464fb6af63c21034c2bc11b45c802ba704dc0ae53a326f13aff049fc0dabfe47ed2090a07f34970c885ffc8679c" },
                { "nl", "b681e1b7e9d430444b95fed8803c549049581b81b631539ee9760efd0602f24823c4b7261f6e1e968d75fe163115820be3a5c58cb6efc132ced486cb49cec4ad" },
                { "nn-NO", "6b1e59fe77132476e7c22704e63881aea4245d52053e3c372260c8190d95cc51bc2cdba84d6a6f9c9f119af5d20f01f42d170e4e853886f6a847a023d8bcb163" },
                { "oc", "5370b3d6c464e75d57823b4a2763c9cc09868f8bf2e91b5e4210e86aa72bba492860dadae7259902ba95323b168555e0fd3fd32a73bbca21c71ef1717fba83a6" },
                { "pa-IN", "7c4f33dc04584bfff7187788d0fc6b0fb22a45927c641af287bd150cfa5eac5d2daa513c59b4a0552bbca2860650b7b47b0e5e307a246a26b0cae74befe13cbc" },
                { "pl", "f61d62f53918efa5a66e15ea7f47333df7559e3fcebe2a09d91e73f307270bdaca8c00b0933e411e4051904e9fff22ebc25e804a732dceaf307def32d7015bfa" },
                { "pt-BR", "536a54da6a7a35f2bafd703252312a108060727801867192530c3c62d53f1a8eba9a81a2f63d5902231d69125908bed35020d65d5b7a63a616a828b6b586214b" },
                { "pt-PT", "fb3f4e7d3ab0af64f4720a32a69caef6b59665a331e5a522f197bfae88bb4911b44d30bdb3971ad036886b35404aa01e2470c5cc3869b8fb4dade62f63fcdf8b" },
                { "rm", "db598720f50ed707ed5dfe7ec6f7fb5baf4bac9c8da788d053b7dba50be7dbaa42a9bfc68957f2c89bce7ae2888ba7957d7e7a37edd49ddd0daf793e58b07cd5" },
                { "ro", "6dcf3882fda0a9d28603ba6d1ce01907f1fab06c921ed65ffdde2116942438986ef6973a78b0f08b19fe3bbacf75a49b814f1362ff3b7461dc0df18d40a92650" },
                { "ru", "af29ee1fb5bac127ad9dc658b549e79070f47dfdf52d78d197ad7ae36f63a1f34a8389ba5731adaac919ee5cc8e8daa5ee7dce5599f851a02823422640b5ab33" },
                { "sco", "95eb3f8d519ae36ee26ad638c7780516abee268d9e73a0aa2714d6810b4a3bf3659a5e83f1d60ded915cde2d2285c046fc248288eaa48fc412fa2e43d06e6ce4" },
                { "si", "ce9433470bd2db00f5e29d0de11d7e2abbb92259857875275ab10c8c5dafd4c1645644f047fb5f8e6c77e102bbc6bbaf4ebadf514c21c980d99f5abfbb4552de" },
                { "sk", "56e9395c71d129ac6aef2c2aea898b66cab9c140b964633bede572ac69ab60c7b631f8f84b6840eb04fea5bfdee49a667876db99af40a53cc7452d2bc6b22d6b" },
                { "sl", "6485f097008ca2dd55c560eb2c34b5af4c84eb1cc531850d6100053aef2f531e9d6c365def08d2f2a8fb6440f65e794345b858016a1670157b2bb3d55f0f14b9" },
                { "son", "a35392b73412a1c8ef8a36b6060161654e580ab8bc95a9a75ab2800fad9648c48900774a6f7da28f11bce00023b33998cdb6ad55384115e6fc044277766b496f" },
                { "sq", "3b1c765ef372d16d3f57e048d341d652eaa8bf60fb5fdd9aa7dff6c609990605f4bcbfbd722373899aa204e06b27abae51032cc7e4ebdfc3d12b32bef1f412a8" },
                { "sr", "f7381fbac719cce138605f9d761150c82b18b206a719ed031c7dc5d8369597f0bb93807edeeb67064f79af759d559aab0cae33613b680af5687338f986328815" },
                { "sv-SE", "fc2baa91344305e19f0a68c6f946ea00a52b8dd4cde8991760cc7fbf89738bd5166a74ada486888468b05009c3f846f8f7b3a108932d7260660a35cbde29b58b" },
                { "szl", "a3a97c2d16e860b4efd61be7e2cc3dd58f6726d5278ec651dcf3613bf115072502e43499a66754d884623100b14a7d4be07ddddccbc2d01152e139c30929c97c" },
                { "ta", "cf34f19bd182e6ca33d2671f675c10c15423840651ce63dcff3c589917c0fecfc13f347c79ea8479c33bc9a8f8eb5ea17aa3292e07a622dba118193df48aec17" },
                { "te", "b0a27c206def767d17cc502b052cbef1c0a17e5b4cef0751b54a72e4183cb9358bf2a80c48712b776f30c1fd1a5173c858372ddd276871f837c76c4ac6f55078" },
                { "th", "9857fe9d0333faa0d1b3dc4b682ed4012821b97abb4b278553814844d1ff8b6548898558d48828cb872d9095c873c32cca21456966be9b4a84ff3621b0193708" },
                { "tl", "83dbaaf1e9388c510c079dfe57a4923f84ff88b2da44af746745a1b14b4acdc4eeef92687a66c6a65650f5d785016e3e4a079e0e80bc1e90bf22f2053364abb5" },
                { "tr", "1b12db421f5dde55268d057f78b4e0c8322a028954fab3409c2a5acb702b9e8e7267d91f01331fae221b6b80e7d71d3f9849321265f8b47ccd66509f76293a6b" },
                { "trs", "cf79f31d0a0b76de545f17542778cc59f50bd73c490a48015cce3e69b80ca6527519c17890dda1df0db314d5cee5111ac251b9d4dc86d669d1a211ec80124f9c" },
                { "uk", "d9b1e4e4fadf09d332ca8a726bcf27f701c0ea3c59b5a6e6078b692e0bac971c645d22040d1853270b5d862e9be59b929cb07c12d4ee698bc4f1e9545a96fa4d" },
                { "ur", "055c3ea7737a77e10addff3f81be4dfe579d089bdccf40243bd4ce52a5305c8c48b29408d46998f2d887bd1365cda5f81e5c5c2746df4141a9a0fc242bc6eb1f" },
                { "uz", "0aa1bb799300a6dd9056f1037c6ee402642f59fecec089b9ed9ae395b0714720a3b306fa2442e865467ddc1ab63ace44f07efc8dd9a5133b662b30ebc6e86b07" },
                { "vi", "3d277672d5f8e5cf67f69ab3cf6cdd63639e664e1cbc3e0a1d857a6c978fc5f3c0b087ca368c8be064977401b704e9ecfb563790f62133c59491929bc97ba7f1" },
                { "xh", "aec078f481f7084b61f87f42955e44c3fe37cc7afb42a330f10b7331648d8441ac9985401c8bf3f4a8070c0782dadebbffe29405f72d8689884dff02919a6d92" },
                { "zh-CN", "f2f82083b1547b0c0b69569481b8f6f43f47e88e4e07333d07d1a2db42d7683e2b9bff9a24d98bb2f2e3df7167a6e02e58a01b11d36d898e865fd1522ec40b63" },
                { "zh-TW", "e8b115a33c63c3fe73a974fabc16ef599b2d2cc6c1f99c3bff707c894476ff3a5cef6059f9b4e5b080dcbda09f28a7a3416acc67a3f4f6564597ff36887ca915" }
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
